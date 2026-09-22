using CSharpFunctionalExtensions;
using DLMS.Tenants.Services.Models.Role.DataIn;
using DLMS.Tenants.Services.Models.Role.DataOut;
using DMS.Shared.Commons;
using DMS.Shared.Constants;

namespace DLMS.Tenants.Services.Interfaces
{
    public interface IRoleService
    {
        Task<Result<string, ResponseError>> Save(RoleDataIn dataIn, Guid currentUserGuid);
        Task<Result<string, ResponseError>> Delete(Guid id);
        Task<Result<RoleCrud, ResponseError>> Get(Guid id);
        Task<Result<PaginationDataOut<RoleDataOut>, ResponseError>> Query(RoleGetAllDataIn dataIn);
        Task<Result<List<RoleSimpleDataOut>, ResponseError>> GetAllForOptions(Guid currentRole);
        Task<Result<string, ResponseError>> GetHashedPermissions(Guid dmsId);
        Task<Result<string, ResponseError>> ChangeStatus(Guid id, UserStatus status);
    }
}
