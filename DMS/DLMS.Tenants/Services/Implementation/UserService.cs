using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Repositories.UOWs;
using DLMS.Tenants.Services.Interfaces;
using DLMS.Tenants.Services.Models.User.DataIn;
using DMS.Shared.Commons;
using DMS.Shared.Config;
using DMS.Shared.Constants;
using DMS.Shared.FileModels;
using DMS.Shared.Handlers.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace DLMS.Tenants.Services.Implementation
{
    public class UserService : IUserService
    {
        readonly ILogger<UserService> _logger;
        readonly IUnitOfWorkTenant _uow;
        private const string ExceptionLogMessage = "{ExceptionMessage}";
        private readonly DmsConfig _dmsConfig;
        IUserContextHandler _userContextHandler;

        public UserService(
            IUnitOfWorkTenant uow,
            ILogger<UserService> logger,
            DmsConfig dmsConfig,
            IUserContextHandler userContextHandler
        )
        {
            _userContextHandler = userContextHandler;
            _dmsConfig = dmsConfig;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<string, ResponseError>> Save(UserDataIn dataIn, Guid currentUserGuid)
        {
            try
            {
                var role = await _uow.GetRoleRepository().GetByDmsIdAsync(dataIn.Role, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (role.IsFailure)
                    return role.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "The given role doesn't exist in the system."
                        )
                        : role.Error;


                var utility = await _uow.GetUtilityRepository().GetByDmsIdAsync(_userContextHandler.GetUtilityFromContext().UtilityId);
                if (utility.IsFailure)
                    return utility.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "The given utility doesn't exist in the system."
                        )
                        : utility.Error;

                var currentUser = await _uow.GetUserRepository().GetByDmsIdAsync(currentUserGuid, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (currentUser.IsFailure)
                    return currentUser.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "Your current user doesn't exist in the system."
                        )
                        : currentUser.Error;

                var result =
                    (dataIn.Id == null)
                        ? await AddUser(dataIn, currentUser.Value, role.Value, utility.Value)
                        : await EditUser(dataIn, role.Value, utility.Value);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        private async Task<Result<string, ResponseError>> AddUser(
            UserDataIn dataIn,
            UserEntity currentUser,
            RoleEntity role, UtilityEntity utility
        )
        {
            try
            {
                var existingUserByEmail = await _uow.GetUserRepository().GetByEmailAllUtilities(dataIn.Email);
                if (existingUserByEmail.IsSuccess)
                    return new ResponseError(
                        ResponseErrorCode.BadRequest,
                        "User with given email already exists."
                    );

                var user = new UserEntity
                {
                    Email = dataIn.Email,
                    FirstName = dataIn.FirstName,
                    MiddleName = dataIn.MiddleName,
                    LastName = dataIn.LastName,
                    LastUpdateTime = DateTime.UtcNow,
                    RoleId = role.Id,
                    Address = dataIn.Address,
                    UtilityId = utility.Id,
                    PhoneNumber = dataIn.PhoneNumber,
                    Image = dataIn.Image,
                    CreatedTime = DateTime.UtcNow,
                    CreatedById = currentUser.Id,
                    Status = UserStatus.Active
                };

                var addResult = await _uow.GetUserRepository().AddAsync(user);
                if (addResult.IsFailure)
                    return addResult.Error;

                var resultComplete = await _uow.CompleteAsync();
                return resultComplete.IsSuccess ? "User successfully created." : resultComplete.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<ValidationStatus<UserXls>, ResponseError>> BulkImportAsync(
            Guid userId,
            List<UserXls> userImport,
            Dictionary<string, Dictionary<string, Guid>> allRegionsWithAreas,
            Dictionary<string, Guid> allRegions,
            bool overrideExisting = true
        )
        {
            var utility = await _uow.GetUtilityRepository().GetByDmsIdAsync(_userContextHandler.GetUtilityFromContext().UtilityId);
            var allUsers = await _uow.GetUserRepository().GetAllDictionary(_userContextHandler.GetUtilityFromContext().UtilityId);
            var allRoles = await _uow.GetRoleRepository().GetAllDictionary(_userContextHandler.GetUtilityFromContext().UtilityId);
            var createdByUser = await _uow.GetUserRepository().GetByDmsIdAsync(userId, _userContextHandler.GetUtilityFromContext().UtilityId);

            try
            {
                var result = await ValidateBulk(userImport, allRegionsWithAreas, allRoles.Value);
                if (result.IsSuccess && result.Value.ValidItemsCount > 0)
                {
                    List<UserEntity> usersToAdd = new List<UserEntity>();
                    List<UserEntity> usersToUpdate = new List<UserEntity>();

                    foreach (var user in result.Value.ValidItems)
                    {
                        if (allUsers.Value.ContainsKey(user.Email))
                        {
                            if (!overrideExisting)
                            {
                                result.Value.AddError(new ErrorDescription()
                                {
                                    SourceError = SourceError.DB,
                                    Identificator = user.Email,
                                    Description = $"User '{user.Email}' already exists in the system.",
                                });
                                continue;
                            }

                            var userFromDb = allUsers.Value[user.Email];

                            userFromDb.FirstName = user.FirstName;
                            userFromDb.MiddleName = user.MiddleName;
                            userFromDb.LastName = user.LastName;
                            userFromDb.RoleId = allRoles.Value[user.RoleName].Id;
                            userFromDb.Address = user.Address;
                            userFromDb.PhoneNumber = user.PhoneNumber;
                            userFromDb.UtilityId = utility.Value.Id;
                            userFromDb.LastUpdateTime = DateTime.UtcNow;



                            usersToUpdate.Add(userFromDb);

                        }
                        else
                        {
                            UserEntity newUser = new UserEntity()
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                MiddleName = user.MiddleName,
                                Email = user.Email,
                                UtilityId = utility.Value.Id,
                                Address = user.Address,
                                PhoneNumber = user.PhoneNumber,
                                RoleId = allRoles.Value[user.RoleName].Id,
                                CreatedTime = DateTime.UtcNow,
                                LastUpdateTime = DateTime.UtcNow,
                                CreatedById = createdByUser.Value.Id,
                                Status = UserStatus.Active
                            };


                            usersToAdd.Add(newUser);

                        }
                    }

                    var savedResult = await _uow.GetUserRepository()
                        .SaveRangeExtension(usersToAdd, usersToUpdate);
                    if (savedResult.IsFailure)
                    {
                        result.Value.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = "Database Error",
                                Description = savedResult.Error.Message,
                            }
                        );
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occured.");
            }
        }

        private async Task<Result<string, ResponseError>> EditUser(UserDataIn dataIn, RoleEntity role, UtilityEntity utility)
        {
            try
            {
                var user = await _uow.GetUserRepository().GetByDmsIdAsync(dataIn.Id.Value, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (user.IsFailure)
                    return new ResponseError(ResponseErrorCode.Database_NotFound, "User not found.");

                var existingUserByEmail = await _uow.GetUserRepository().GetByEmailAllUtilities(dataIn.Email);
                if (existingUserByEmail.IsSuccess && user.Value.Id != existingUserByEmail.Value.Id)
                    return new ResponseError(
                        ResponseErrorCode.BadRequest,
                        "User with given email already exists."
                    );

                user.Value.Email = dataIn.Email;
                user.Value.FirstName = dataIn.FirstName;
                user.Value.MiddleName = dataIn.MiddleName;
                user.Value.LastName = dataIn.LastName;
                user.Value.RoleId = role.Id;
                user.Value.Address = dataIn.Address;
                user.Value.PhoneNumber = dataIn.PhoneNumber;
                user.Value.Image = dataIn.Image;
                user.Value.LastUpdateTime = DateTime.UtcNow;
                user.Value.LastUpdateTime = DateTime.UtcNow;

                var resultComplete = await _uow.CompleteAsync();
                return resultComplete.IsSuccess ? "User successfully updated." : resultComplete.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<string, ResponseError>> EditProfile(UserEditDataIn dataIn)
        {
            try
            {
                var user = await _uow.GetUserRepository().GetByDmsIdAsync(dataIn.Id.Value, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (user.IsFailure)
                    return new ResponseError(ResponseErrorCode.Database_NotFound, "User not found.");

                user.Value.FirstName = dataIn.FirstName;
                user.Value.MiddleName = dataIn.MiddleName;
                user.Value.LastName = dataIn.LastName;
                user.Value.Address = dataIn.Address;
                user.Value.PhoneNumber = dataIn.PhoneNumber;
                user.Value.Image = dataIn.Image;
                user.Value.LastUpdateTime = DateTime.UtcNow;


                var resultComplete = await _uow.CompleteAsync();
                return resultComplete.IsSuccess ? "User successfully updated." : resultComplete.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<string, ResponseError>> Delete(Guid id)
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().GetByDmsIdAsync(id, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Role!);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;
                else if (userFromDb.Value.Role?.Name.Trim().ToLower() == "superadmin")
                    return new ResponseError(ResponseErrorCode.BadRequest, "The SuperAdmin cannot be deleted from the system. Please contact the administrator to process your request.");

                var removeRet = _uow.GetUserRepository().Remove(userFromDb.Value);
                if (removeRet.IsFailure)
                    return removeRet.Error;
                else
                {
                    var completeSave = await _uow.CompleteAsync();
                    if (completeSave.IsFailure)
                        return completeSave.Error;
                }
                return "Successfully removed.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<UserDataOut, ResponseError>> GetByEmail(string email)
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().GetByEmailAllUtilities(email);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;

                return new UserDataOut(userFromDb.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<UserDataOut, ResponseError>> GetByGuid(Guid dmsId)
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().GetByDmsIdAllUtilitiesAsync(dmsId, x => x.Role!, x => x.CreatedBy, x => x.Utility);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;

                return new UserDataOut(userFromDb.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<List<UserDataOut>, ResponseError>> GetByGuids(List<Guid> dmsIds)
        {
            try
            {
                var usersFromDb = await _uow.GetUserRepository().GetByGuids(dmsIds, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (usersFromDb.IsFailure)
                    return usersFromDb.Error;

                var result = usersFromDb.Value.Select(user => new UserDataOut(user)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }


        /// <summary>
        /// Retrieves the list of permissions for a DMS user.
        /// </summary>
        /// <param name="dmsId">The unique identifier of the DMS user.</param>
        /// <param name="desktopUser">
        ///     <c>null</c> – returns permissions for all user types.<br/>
        ///     <c>true</c> – returns permissions for desktop users only.<br/>
        ///     <c>false</c> – returns permissions for web application users only.
        /// </param>
        /// <returns>
        /// A <see cref="Result{T, TError}"/> containing a list of permissions
        /// (<see cref="PermissionsEnum"/>) or an error (<see cref="ResponseError"/>).
        /// </returns>
        public async Task<Result<List<PermissionsEnum>, ResponseError>> GetPermissions(Guid dmsId, bool? desktopUser = null)
        {
            try
            {
                var userPermissionsFromDb = await _uow.GetUserRepository().GetPermissions(dmsId, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (userPermissionsFromDb.IsFailure)
                {
                    return userPermissionsFromDb.Error;
                }
                else if (userPermissionsFromDb.Value.Count == 0)
                {
                    return new ResponseError()
                    {
                        ErrorCode = ResponseErrorCode.Database_NotFound,
                        Message = "No permission for requested user.",
                    };
                }

                var permissions = userPermissionsFromDb.Value.Select(p => p.Value).ToList();

                if (permissions.Count == 0)
                {
                    return new ResponseError()
                    {
                        ErrorCode = ResponseErrorCode.Database_NotFound,
                        Message = "No permission for requested user.",
                    };
                }
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<PaginationDataOut<UserDataOut>, ResponseError>> Query(
            UserGetAllDataIn dataIn
        )
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().Query(dataIn, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;

                return new PaginationDataOut<UserDataOut>()
                {
                    Count = userFromDb.Value.Count,
                    Data = userFromDb.Value.Data.Select(x => new UserDataOut(x)).ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<string, ResponseError>> ChangeStatus(Guid id, UserStatus status)
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().GetByDmsIdAsync(id, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;

                userFromDb.Value.Status = status;
                userFromDb.Value.LastUpdateTime = DateTime.UtcNow;


                var response = await _uow.CompleteAsync();
                return response.IsSuccess ? "User status successfully updated." : response.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }


        public async Task<Result<ValidationStatus<UserXls>, ResponseError>> ValidateBulk(
            List<UserXls> users,
            Dictionary<string, Dictionary<string, Guid>> allRegionsWithAreas,
            Dictionary<string, RoleEntity> allRoles
        )
        {
            try
            {
                var retVal = new ValidationStatus<UserXls>() { };

                foreach (var user in users)
                {
                    var errorCount = retVal.ErrorList.Count;

                    var hasDuplicate = users.Count(u => u.Email == user.Email) > 1;

                    if (hasDuplicate)
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                Description =
                                    $"The file contains multiple users with the same email {user.Email}",
                                SourceError = SourceError.DB,
                                Identificator = user.Email.ToString(),
                            }
                        );

                    if (string.IsNullOrEmpty(user.FirstName))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = user.Email,
                                Description = "First Name is mandatory field!",
                            }
                        );
                    }
                    if (string.IsNullOrEmpty(user.LastName))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = user.Email,
                                Description = "Last Name is mandatory field!",
                            }
                        );
                    }
                    if (string.IsNullOrEmpty(user.Email))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = user.Email,
                                Description = "Email is mandatory field!",
                            }
                        );
                    }
                    if (string.IsNullOrEmpty(user.RoleName))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = user.Email,
                                Description = "Role Name is mandatory field!",
                            }
                        );
                    }
                    else if (!allRoles.ContainsKey(user.RoleName))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.DB,
                                Identificator = user.RoleName,
                                Description = "Role Name doesn't exist in our system.",
                            }
                        );
                    }
                    if (!Regex.IsMatch(user.Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.None, TimeSpan.FromMilliseconds(200)))
                    {
                        retVal.AddError(
                            new ErrorDescription()
                            {
                                SourceError = SourceError.File,
                                Identificator = user.Email,
                                Description = "Email is not valid.",
                            }
                        );
                    }
                    if (errorCount == retVal.ErrorList.Count)
                        retVal.ValidItems.Add(user);
                }
                return retVal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occured.");
            }
        }

        public async Task<Result<bool, ResponseError>> UpdateLastLogin(Guid dmsId)
        {
            try
            {
                var userFromDb = await _uow.GetUserRepository().GetByDmsIdAsync(dmsId, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (userFromDb.IsFailure)
                    return userFromDb.Error;

                userFromDb.Value.LastUpdateTime = DateTime.UtcNow;

                var response = await _uow.CompleteAsync();
                if (response.IsFailure)
                    return response.Error;

                return response.IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<string, ResponseError>> AddForUtility(UserDataIn dataIn, Guid currentUserGuid, Guid utilityGuid)
        {
            try
            {
                var role = await _uow.GetRoleRepository().GetByDmsIdAsync(dataIn.Role, utilityGuid);
                if (role.IsFailure)
                    return role.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "The given role doesn't exist in the system."
                        )
                        : role.Error;


                var utility = await _uow.GetUtilityRepository().GetByDmsIdAsync(utilityGuid);
                if (utility.IsFailure)
                    return utility.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "The given utility doesn't exist in the system."
                        )
                        : utility.Error;

                var currentUser = await _uow.GetUserRepository().GetByDmsIdAllUtilitiesAsync(currentUserGuid);
                if (currentUser.IsFailure)
                    return currentUser.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "Your current user doesn't exist in the system."
                        )
                        : currentUser.Error;

                return await AddUser(dataIn, currentUser.Value, role.Value, utility.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionLogMessage, ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }
    }
}
