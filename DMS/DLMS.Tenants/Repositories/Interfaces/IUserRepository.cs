using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.User.DataIn;
using DMS.Shared.Commons;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<Result<UserEntity, ResponseError>> GetByEmailAllUtilities(string email);
        Task<Result<List<UserEntity>, ResponseError>> GetByGuids(List<Guid> dmsIds, Guid utilityId);
        Task<Result<UserEntity, ResponseError>> GetByDmsIdAsync(Guid dmsId, Guid utilityId, params Expression<Func<UserEntity, object>>[] includes);
        Task<Result<UserEntity, ResponseError>> GetByDmsIdAllUtilitiesAsync(Guid dmsId, params Expression<Func<UserEntity, object>>[] includes);
        Task<Result<UserEntity, ResponseError>> AddAsync(UserEntity user);
        Task<Result<List<PermissionEntity>, ResponseError>> GetPermissions(Guid dmsId, Guid utilityId);
        Task<Result<PaginationDataOut<UserEntity>, ResponseError>> Query(UserGetAllDataIn dataIn, Guid utilityGuid);
        Task<Result<Dictionary<string, UserEntity>, ResponseError>> GetAllDictionary(Guid utilityId);
        Task<Result<string, ResponseError>> SaveRangeExtension(
            List<UserEntity> addUsers,
            List<UserEntity> updateUsers
        );
        Result<bool, ResponseError> Remove(UserEntity entity);
    }
}
