using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Utility.DataIn;
using DLMS.Tenants.Services.Models.Utility.DataOut;
using DMS.Shared.Commons;

namespace DLMS.Tenants.Services.Interfaces
{
    public interface IUtilityService
    {
        Task<Result<List<UtilityDto>, ResponseError>> GetAll();
        Task<Result<PaginationDataOut<UtilityDto>, ResponseError>> Query(UtilityGetAllDataIn dataIn);
        Task<Result<UtilityEntity, ResponseError>> Save(CreateUtilityDataIn dataIn, Guid user, string filePath);
        Task<Result<UtilityDto, ResponseError>> GetById(Guid id);
        Task<Result<Guid, ResponseError>> GetApiKey(Guid id);
        Task<Result<string, ResponseError>> GetAcronymByIdCached(Guid id);
        Task<Result<UtilityDto, ResponseError>> GetByName(string name);
        Task<Result<UtilityDto, ResponseError>> GetByAcronym(string acronym);
        Task<Result<Guid, ResponseError>> Edit(EditUtilityDataIn dataIn, Guid user, string? filePath = null);
        Task<Result<Guid, ResponseError>> UpdateStatus(UtilityStatusDataIn dataIn);
    }
}
