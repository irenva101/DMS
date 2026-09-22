using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Repositories.UOWs;
using DLMS.Tenants.Services.Interfaces;
using DLMS.Tenants.Services.Models.Role.DataIn;
using DLMS.Tenants.Services.Models.Role.DataOut;
using DMS.Shared.Commons;
using DMS.Shared.Constants;
using DMS.Shared.Handlers.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace DLMS.Tenants.Services.Implementation
{
    public class RoleService : IRoleService
    {
        readonly ILogger<RoleService> _logger;
        readonly IUnitOfWorkTenant _uow;
        readonly IUserContextHandler _userContextHandler;

        public RoleService(IUnitOfWorkTenant uow, ILogger<RoleService> logger, IUserContextHandler userContextHandler)
        {
            _userContextHandler = userContextHandler;
            _uow = uow;
            _logger = logger;
        }

        public async Task<Result<string, ResponseError>> Save(RoleDataIn dataIn, Guid currentUserGuid)
        {
            try
            {
                var currentUser = await _uow.GetUserRepository().GetByDmsIdAsync(currentUserGuid, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Utility);
                if (currentUser.IsFailure)
                    return currentUser.Error.ErrorCode == ResponseErrorCode.Database_NotFound
                        ? new ResponseError(
                            ResponseErrorCode.Database_NotFound,
                            "Your current user doesn't exist in the system."
                        )
                        : currentUser.Error;

                return dataIn.Id == null
                    ? await AddRole(dataIn, currentUser.Value)
                    : await EditRole(dataIn);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        private async Task<Result<string, ResponseError>> AddRole(
            RoleDataIn dataIn,
            UserEntity currentUser
        )
        {
            var existingUserByEmail = await _uow.GetRoleRepository().GetByName(dataIn.Name, _userContextHandler.GetUtilityFromContext().UtilityId);
            if (existingUserByEmail.IsSuccess)
                return new ResponseError(
                    ResponseErrorCode.BadRequest,
                    "User with given name already exists."
                );

            var newRole = new RoleEntity
            {
                Name = dataIn.Name,
                LastUpdateTime = DateTime.UtcNow,
                Permissions = new List<PermissionEntity>(),
                CreatedTime = DateTime.UtcNow,
                CreatedById = currentUser.Id,
                UtilityId = currentUser.Utility != null ? currentUser.Utility.Id : 0
            };

            if (dataIn.Permissions != null)
            {
                var permissionsFromDb = await _uow.GetPermissionRepository()
                    .GetPermissionsForListValues(dataIn.Permissions);

                if (permissionsFromDb.IsFailure)
                    return permissionsFromDb.Error;

                newRole.Permissions = permissionsFromDb.Value; //set perms
            }

            var addResult = await _uow.GetRoleRepository().AddAsync(newRole);
            if (addResult.IsFailure)
                return addResult.Error;

            var resultComplete = await _uow.CompleteAsync();
            return resultComplete.IsSuccess ? "Role successfully created." : resultComplete.Error;
        }

        private async Task<Result<string, ResponseError>> EditRole(RoleDataIn dataIn)
        {
            var role = await _uow.GetRoleRepository().GetByDmsIdAsync(dataIn.Id.Value, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Permissions);
            if (role.IsFailure)
                return new ResponseError(ResponseErrorCode.Database_NotFound, "Role not found.");

            if (role.Value.Name.Trim().ToLower() == "superadmin")
                return new ResponseError(ResponseErrorCode.Database_NotFound, "Unable to edit the SuperAdmin role.");

            var existingRoleByName = await _uow.GetRoleRepository().GetByName(dataIn.Name, _userContextHandler.GetUtilityFromContext().UtilityId);
            if (existingRoleByName.IsSuccess && dataIn.Id != existingRoleByName.Value.DmsId)
                return new ResponseError(
                    ResponseErrorCode.BadRequest,
                    "Role with given name already exists."
                );

            role.Value.Name = dataIn.Name;
            role.Value.LastUpdateTime = DateTime.UtcNow;
            if (dataIn.Permissions != null)
            {
                var permissionsFromDb = await _uow.GetPermissionRepository()
                    .GetPermissionsForListValues(dataIn.Permissions);

                if (permissionsFromDb.IsFailure)
                    return permissionsFromDb.Error;

                role.Value.Permissions = permissionsFromDb.Value; //set perms
            }

            var resultComplete = await _uow.CompleteAsync();
            return resultComplete.IsSuccess ? "Role successfully updated." : resultComplete.Error;
        }

        public async Task<Result<string, ResponseError>> Delete(Guid id)
        {
            try
            {
                var roleFromDb = await _uow.GetRoleRepository().GetByDmsIdAsync(id, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Users);
                if (roleFromDb.IsFailure)
                    return roleFromDb.Error;
                else if (roleFromDb.Value.Users.Count > 0)
                    return new ResponseError(
                        ResponseErrorCode.BadRequest,
                        "You cannot delete a role that has users assigned to it."
                    );
                else if (roleFromDb.Value.Name.Trim().ToLower() == "superadmin")
                    return new ResponseError(
                        ResponseErrorCode.BadRequest,
                        "You cannot delete the SuperAdmin role."
                    );

                var removeRet = _uow.GetRoleRepository().Remove(roleFromDb.Value);
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
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<PaginationDataOut<RoleDataOut>, ResponseError>> Query(
            RoleGetAllDataIn dataIn
        )
        {
            try
            {
                var roleFromDb = await _uow.GetRoleRepository().Query(dataIn, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (roleFromDb.IsFailure)
                    return roleFromDb.Error;

                return new PaginationDataOut<RoleDataOut>()
                {
                    Count = roleFromDb.Value.Count,
                    Data = roleFromDb.Value.Data.Select(x => new RoleDataOut(x)).ToList(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<List<RoleSimpleDataOut>, ResponseError>> GetAllForOptions(Guid currentRole)
        {
            try
            {
                var roles = await _uow.GetRoleRepository().GetAllAsync(_userContextHandler.GetUtilityFromContext().UtilityId);
                if (roles.IsFailure)
                    return roles.Error;

                bool isCurrentUserSuperAdmin =
                roles.Value.Any(x =>
                    x.DmsId == currentRole &&
                    x.Name.Trim().Equals("superadmin", StringComparison.OrdinalIgnoreCase));

                var filtered = roles.Value.Where(x =>
                {
                    var isSuperAdminRole = x.Name.Trim().Equals("superadmin", StringComparison.OrdinalIgnoreCase);

                    // if NOT a superadmin user → always hide it
                    if (!isCurrentUserSuperAdmin && isSuperAdminRole)
                        return false;

                    // if superadmin does not exist in the database → nothing special
                    return true;
                });

                return filtered
                    .Select(x => new RoleSimpleDataOut(x))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<RoleCrud, ResponseError>> Get(Guid id)
        {
            try
            {
                var roleFromDb = await _uow.GetRoleRepository().GetByDmsIdAsync(id, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Permissions);
                if (roleFromDb.IsFailure)
                    return roleFromDb.Error;

                return new RoleCrud(roleFromDb.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<string, ResponseError>> ChangeStatus(Guid id, UserStatus status)
        {
            try
            {
                var roleFromDb = await _uow.GetRoleRepository().GetByDmsIdAsync(id, _userContextHandler.GetUtilityFromContext().UtilityId);
                if (roleFromDb.IsFailure)
                    return roleFromDb.Error;

                roleFromDb.Value.Status = status;
                roleFromDb.Value.LastUpdateTime = DateTime.UtcNow;

                var response = await _uow.CompleteAsync();
                return response.IsSuccess ? "Role status successfully updated." : response.Error;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return DefaultErrorResponses.ExceptionError;
            }
        }

        public async Task<Result<string, ResponseError>> GetHashedPermissions(Guid dmsId)
        {
            try
            {
                var userPermissionsFromDb = await _uow.GetRoleRepository().GetByDmsIdAsync(dmsId, _userContextHandler.GetUtilityFromContext().UtilityId, x => x.Permissions);
                if (userPermissionsFromDb.IsFailure)
                    return userPermissionsFromDb.Error;
                else if (userPermissionsFromDb.Value.Permissions.Count == 0)
                {
                    return new ResponseError()
                    {
                        ErrorCode = ResponseErrorCode.BadRequest,
                        Message = "No permissions for requested user.",
                    };
                }

                var permissions = userPermissionsFromDb.Value.Permissions.Where(x => !x.Value.ToString().StartsWith("desktop_")).Select(p => p.Value).ToList();
                if (permissions.Count == 0)
                {
                    return new ResponseError()
                    {
                        ErrorCode = ResponseErrorCode.BadRequest,
                        Message = "No permissions for requested user.",
                    };
                }
                return Base64UrlEncoder.Encode(JsonSerializer.Serialize(permissions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }
    }
}
