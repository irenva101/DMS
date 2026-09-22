using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Repositories.UOWs;
using DLMS.Tenants.Services.Interfaces;
using DLMS.Tenants.Services.Models.User.DataIn;
using DLMS.Tenants.Services.Models.Utility.DataIn;
using DLMS.Tenants.Services.Models.Utility.DataOut;
using DMS.Shared.Commons;
using DMS.Shared.Constants;
using DMS.Shared.Handlers.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DLMS.Tenants.Services.Implementation
{
    public class UtilityService(
        IUnitOfWorkTenant _uow,
        IUserService _userService,
        ILogger<UtilityService> _logger,
        IRoleService _roleService,
        IUserContextHandler _userContextHandler,
        IServiceScopeFactory _scopeFactory,
        IMemoryCache _memoryCache) : IUtilityService
    {
        public async Task<Result<List<UtilityDto>, ResponseError>> GetAll()
        {
            try
            {
                var utilities = await _uow.GetUtilityRepository().GetAll();
                if (utilities.IsFailure)
                    return utilities.Error;

                return utilities.Value.Select(x => new UtilityDto(x)).ToList();
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<UtilityDto, ResponseError>> GetById(Guid id)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByDmsIdAsync(id, x => x.ContactPerson);
                if (utilityFromDb.IsFailure)
                    return utilityFromDb.Error;
                return new UtilityDto(utilityFromDb.Value);
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<Guid, ResponseError>> GetApiKey(Guid id)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByDmsIdAsync(id, x => x.ContactPerson);
                if (utilityFromDb.IsFailure)
                    return utilityFromDb.Error;
                return utilityFromDb.Value.DmsId;
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<string, ResponseError>> GetAcronymByIdCached(Guid id)
        {
            try
            {
                var cacheKey = GetUtilityCacheKey(id);

                if (_memoryCache.TryGetValue(cacheKey, out string? cachedUtilityAcronym) && !string.IsNullOrWhiteSpace(cachedUtilityAcronym))
                    return cachedUtilityAcronym;

                var utilityFromDb = await GetById(id);
                if (utilityFromDb.IsFailure)
                    return utilityFromDb.Error;

                var utilityAcronym = utilityFromDb.Value.Acronym;

                _memoryCache.Set(
                    cacheKey,
                    utilityAcronym,
                    new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });

                return utilityAcronym;
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<PaginationDataOut<UtilityDto>, ResponseError>> Query(UtilityGetAllDataIn dataIn)
        {
            try
            {
                var utilities = await _uow.GetUtilityRepository().Query(dataIn);
                if (utilities.IsFailure)
                    return utilities.Error;

                return new PaginationDataOut<UtilityDto>()
                {
                    Count = utilities.Value.Count,
                    Data = utilities.Value.Data.Select(x => new UtilityDto(x)).ToList()
                };
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<UtilityEntity, ResponseError>> Save(CreateUtilityDataIn dataIn, Guid user, string filePath)
        {
            try
            {
                //validate
                var currentUserFromDb = await _uow.GetUserRepository().GetByDmsIdAllUtilitiesAsync(user);
                if (currentUserFromDb.IsFailure)
                    return currentUserFromDb.Error;

                var userFromDb = await _uow.GetUserRepository().GetByEmailAllUtilities(dataIn.ContactPersonEmail);
                if (userFromDb.IsSuccess || (userFromDb.IsFailure && userFromDb.Error.ErrorCode != ResponseErrorCode.Database_NotFound))
                    return new ResponseError(ResponseErrorCode.BadRequest, "Contact person already exists in the system, please enter a different email.");

                var utilityAcronymFromDb = await _uow.GetUtilityRepository().GetByAcronym(dataIn.Acronym);
                if (utilityAcronymFromDb.IsSuccess || (utilityAcronymFromDb.IsFailure && utilityAcronymFromDb.Error.ErrorCode != ResponseErrorCode.Database_NotFound))
                    return new ResponseError(ResponseErrorCode.BadRequest, "Utility with given acronym already exists in the system.");

                var utilityNameFromDb = await _uow.GetUtilityRepository().GetByName(dataIn.Name);
                if (utilityNameFromDb.IsSuccess || (utilityNameFromDb.IsFailure && utilityNameFromDb.Error.ErrorCode != ResponseErrorCode.Database_NotFound))
                    return new ResponseError(ResponseErrorCode.BadRequest, "Utility with given name already exists in the system.");


                //add utility
                var utility = new UtilityEntity()
                {
                    Name = dataIn.Name,
                    ApiKey = Guid.NewGuid(),
                    Acronym = dataIn.Acronym,
                    LastUpdateTime = DateTime.UtcNow,
                    CreationDate = DateTime.UtcNow,
                    WebsiteAddress = dataIn.WebsiteAddress,
                    Address = dataIn.Address,
                    Status = DMS.Shared.Constants.UtilityStatus.Active,
                    LogoPath = filePath,
                    TimeZone = dataIn.TimeZone
                };

                var utilityReponse = await _uow.GetUtilityRepository().AddAsync(utility);
                if (utilityReponse.IsFailure)
                    return utilityReponse.Error;

                var completeRetUtility = await _uow.CompleteAsync();
                if (completeRetUtility.IsFailure)
                    return completeRetUtility.Error;

                //permissions
                var allPermissions = await _uow.GetPermissionRepository().GetAllAvailablePermissions();
                if (allPermissions.IsFailure)
                    return allPermissions.Error;

                //utility admin role
                var role = new RoleEntity()
                {
                    Name = "SuperAdmin",
                    Permissions = allPermissions.Value,
                    UtilityId = utility.Id,
                    CreatedTime = DateTime.UtcNow,
                    CreatedById = currentUserFromDb.Value.Id,
                    Status = UserStatus.Active
                };
                var roleResponse = await _uow.GetRoleRepository().AddAsync(role);
                if (roleResponse.IsFailure)
                    return roleResponse.Error;
                var completeRetRole = await _uow.CompleteAsync();
                if (completeRetRole.IsFailure)
                    return completeRetRole.Error;

                //add contact person
                var userSave = await _userService.AddForUtility(new UserDataIn()
                {
                    Id = null,
                    FirstName = dataIn.ContactPersonFirstName,
                    LastName = dataIn.ContactPersonLastName,
                    Email = dataIn.ContactPersonEmail,
                    Role = role.DmsId,
                    Address = null,
                    PhoneNumber = dataIn.ContactPersonPhoneNumber,
                    Image = null,
                }, user, utility.DmsId);
                if (userSave.IsFailure)
                    return userSave.Error;

                var userFromDb2 = await _uow.GetUserRepository().GetByEmailAllUtilities(dataIn.ContactPersonEmail);
                if (userFromDb2.IsFailure)
                    return new ResponseError(ResponseErrorCode.BadRequest, "The user was not successfully saved to the database.");

                //set id on this utility
                utility.ContactPersonId = userFromDb2.Value.Id;
                utility.LastUpdateTime = DateTime.UtcNow;
                var completeResponse = await _uow.CompleteAsync();
                if (completeResponse.IsFailure)
                    return completeResponse.Error;

                //send email
                //var contentEmail = "Dear " + userFromDb2.Value.FirstName + " " + userFromDb2.Value.LastName + " ,<br><br>" +
                //                    "Your utility named <strong>" + utility.Name + "</strong> has been successfully created.<br>" +
                //                    "You will soon receive an email with your access credentials.<br><br>" +
                //                    "If you need any additional information or assistance, please don’t hesitate to contact us.<br><br>" +
                //                    "Best regards,<br>" +
                //                    "BPS DMS Team";
                //var response = await _emailService.SendMailAsync(new EmailData(userFromDb2.Value.Email, "Utility Successfully Created – " + utility.Name, contentEmail, true));
                //if (response.IsFailure)
                //    return new ResponseError(ResponseErrorCode.Exception, "An error occurred while trying to send an email to the contact person.");

                return utility;
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }


        public async Task<Result<Guid, ResponseError>> Edit(EditUtilityDataIn dataIn, Guid user, string? filePath = null)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByDmsIdAsync(dataIn.Id);
                if (utilityFromDb.IsFailure)
                    return new ResponseError(ResponseErrorCode.BadRequest, utilityFromDb.Error.Message);

                //validate
                var currentUserFromDb = await _uow.GetUserRepository().GetByDmsIdAllUtilitiesAsync(user);
                if (currentUserFromDb.IsFailure)
                    return currentUserFromDb.Error;

                var utilityNameFromDb = await _uow.GetUtilityRepository().GetByName(dataIn.Name);
                if (utilityNameFromDb.IsFailure && utilityNameFromDb.Error.ErrorCode != ResponseErrorCode.Database_NotFound)
                    return new ResponseError(ResponseErrorCode.BadRequest, utilityNameFromDb.Error.Message);

                if (utilityNameFromDb.IsSuccess && utilityFromDb.Value.Name != dataIn.Name)
                    return new ResponseError(ResponseErrorCode.BadRequest, "Utility with given name already exists in the system.");

                utilityFromDb.Value.Address = dataIn.Address;
                utilityFromDb.Value.WebsiteAddress = dataIn.WebsiteAddress;
                utilityFromDb.Value.Name = dataIn.Name;
                utilityFromDb.Value.LastUpdateTime = DateTime.UtcNow;
                utilityFromDb.Value.TimeZone = dataIn.TimeZone;
                if (filePath != null) utilityFromDb.Value.LogoPath = filePath;

                var completeRetUtility = await _uow.CompleteAsync();
                if (completeRetUtility.IsFailure)
                    return completeRetUtility.Error;

                return utilityFromDb.Value.DmsId;
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<Guid, ResponseError>> UpdateStatus(UtilityStatusDataIn dataIn)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByDmsIdAsync(dataIn.UtilityId);
                if (utilityFromDb.IsFailure)
                    return new ResponseError(ResponseErrorCode.BadRequest, utilityFromDb.Error.Message);

                utilityFromDb.Value.Status = dataIn.Status;
                utilityFromDb.Value.LastUpdateTime = DateTime.UtcNow;

                var completeRetUtility = await _uow.CompleteAsync();
                if (completeRetUtility.IsFailure)
                    return completeRetUtility.Error;

                return utilityFromDb.Value.DmsId;
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<UtilityDto, ResponseError>> GetByName(string name)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByName(name);
                if (utilityFromDb.IsFailure)
                    return utilityFromDb.Error;
                return new UtilityDto(utilityFromDb.Value);
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        public async Task<Result<UtilityDto, ResponseError>> GetByAcronym(string acronym)
        {
            try
            {
                var utilityFromDb = await _uow.GetUtilityRepository().GetByAcronym(acronym);
                if (utilityFromDb.IsFailure)
                    return utilityFromDb.Error;
                return new UtilityDto(utilityFromDb.Value);
            }
            catch (Exception ex)
            {
                return new ResponseError(ResponseErrorCode.Exception, ex.Message);
            }
        }

        private static string GetUtilityCacheKey(Guid dmsId) => $"utility:{dmsId}";
    }
}
