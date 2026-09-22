using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Utility.DataIn;
using DMS.Shared.Commons;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Interfaces
{
    public interface IUtilityRepository
    {
        Task<Result<List<UtilityEntity>, ResponseError>> GetAll();
        Task<Result<PaginationDataOut<UtilityEntity>, ResponseError>> Query(UtilityGetAllDataIn dataIn);
        Task<Result<UtilityEntity, ResponseError>> GetByAcronym(string data);
        Task<Result<UtilityEntity, ResponseError>> GetByName(string data);
        Task<Result<UtilityEntity, ResponseError>> GetByDmsIdAsync(Guid id, params Expression<Func<UtilityEntity, object>>[] includes);
        Task<Result<UtilityEntity, ResponseError>> GetByApiKey(Guid apikey);
        Task<Result<UtilityEntity, ResponseError>> AddAsync(UtilityEntity utility);
        Result<bool, ResponseError> Remove(UtilityEntity entity);
    }
}
