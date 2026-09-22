using CSharpFunctionalExtensions;
using DLMS.Tenants.Services.Models.User.DataIn;
using DMS.Shared.Commons;
using DMS.Shared.Constants;
using DMS.Shared.FileModels;

namespace DLMS.Tenants.Services.Interfaces
{
    public interface IUserService
    {
        Task<Result<string, ResponseError>> AddForUtility(UserDataIn dataIn, Guid currentUserGuid, Guid utilityGuid);
        Task<Result<string, ResponseError>> Save(UserDataIn dataIn, Guid currentUserGuid);
        Task<Result<string, ResponseError>> Delete(Guid id);
        Task<Result<string, ResponseError>> ChangeStatus(Guid id, UserStatus status);
        Task<Result<UserDataOut, ResponseError>> GetByEmail(string email);
        Task<Result<UserDataOut, ResponseError>> GetByGuid(Guid dmsId);
        Task<Result<List<UserDataOut>, ResponseError>> GetByGuids(List<Guid> dmsIds);
        Task<Result<ValidationStatus<UserXls>, ResponseError>> BulkImportAsync(
                Guid userId,
                List<UserXls> userImport,
                Dictionary<string, Dictionary<string, Guid>> allRegionsWithAreas,
                Dictionary<string, Guid> allRegions,
                bool overrideExisting = true
            );
        Task<Result<List<PermissionsEnum>, ResponseError>> GetPermissions(Guid dmsId, bool? desktopUser = null);
        Task<Result<PaginationDataOut<UserDataOut>, ResponseError>> Query(UserGetAllDataIn dataIn);
        Task<Result<string, ResponseError>> EditProfile(UserEditDataIn dataIn);
        Task<Result<bool, ResponseError>> UpdateLastLogin(Guid dmsId);
    }
}
