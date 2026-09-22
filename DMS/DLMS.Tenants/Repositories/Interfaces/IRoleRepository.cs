using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Role.DataIn;
using DMS.Shared.Commons;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<Result<RoleEntity, ResponseError>> GetByName(string roleName, Guid utilityId);
        Task<Result<RoleEntity, ResponseError>> GetByDmsIdAsync(Guid id, Guid utilityId, params Expression<Func<RoleEntity, object>>[] includes);
        Task<Result<RoleEntity, ResponseError>> AddAsync(RoleEntity role);
        Task<Result<PaginationDataOut<RoleEntity>, ResponseError>> Query(RoleGetAllDataIn dataIn, Guid utilityId);
        Task<Result<IEnumerable<RoleEntity>, ResponseError>> GetAllAsync(Guid utilityId,
            params Expression<Func<RoleEntity, object>>[] includes
        );
        Result<bool, ResponseError> Remove(RoleEntity entity);
        Task<Result<Dictionary<string, RoleEntity>, ResponseError>> GetAllDictionary(Guid utilityId);
    }
}
