using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Permissions.DataIn;
using DMS.Shared.Commons;

namespace DLMS.Tenants.Repositories.Interfaces
{
    public interface IPermissionRepository
    {
        Task<Result<List<PermissionEntity>, ResponseError>> GetAllAvailablePermissions();
        Task<Result<List<PermissionEntity>, ResponseError>> GetPermissionsForListValues(
            List<PermissionsEnum> perms
        );

        Task<Result<PaginationDataOut<PermissionEntity>, ResponseError>> Query(
            PermissionGetAllDataIn dataIn
        );
    }
}
