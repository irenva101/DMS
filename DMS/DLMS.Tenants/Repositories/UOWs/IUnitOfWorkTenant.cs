using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Interfaces;
using DMS.Shared.Commons;

namespace DLMS.Tenants.Repositories.UOWs
{
    public interface IUnitOfWorkTenant : IDisposable, IAsyncDisposable
    {
        public Task<Result<int, ResponseError>> CompleteAsync();
        public IUtilityRepository GetUtilityRepository();
        public IUserRepository GetUserRepository();
        public IRoleRepository GetRoleRepository();
        public IPermissionRepository GetPermissionRepository();
    }
}
