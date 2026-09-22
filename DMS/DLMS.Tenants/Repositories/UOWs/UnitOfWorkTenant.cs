using CSharpFunctionalExtensions;
using DLMS.Tenants.Data;
using DLMS.Tenants.Repositories.Implementations;
using DLMS.Tenants.Repositories.Interfaces;
using DMS.Shared.Commons;
using Microsoft.Extensions.Logging;

namespace DLMS.Tenants.Repositories.UOWs
{
    public class UnitOfWorkTenant(
        SharedContext _context,
        ILogger<UnitOfWorkTenant> _logger,
        ILogger<UserRepository> _loggerUserRepository,
        ILogger<PermissionRepository> _loggerPermissionRepository,
        ILogger<RoleRepository> _loggerRoleRepository,
        ILogger<UtilityRepository> _loggerUtilityRepository
    ) : IUnitOfWorkTenant
    {
        private IUtilityRepository UtilityRepository { get; set; }
        private IUserRepository UserRepository { get; set; }
        private IRoleRepository RoleRepository { get; set; }
        private IPermissionRepository PermissionRepository { get; set; }

        public async Task<Result<int, ResponseError>> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public IUtilityRepository GetUtilityRepository()
        {
            return UtilityRepository
                ?? (UtilityRepository = new UtilityRepository(_context, _loggerUtilityRepository));
        }

        public IUserRepository GetUserRepository()
        {
            return UserRepository
                ?? (UserRepository = new UserRepository(_context, _loggerUserRepository));
        }

        public IPermissionRepository GetPermissionRepository()
        {
            return PermissionRepository
                ?? (PermissionRepository = new PermissionRepository(_context, _loggerPermissionRepository));
        }

        public IRoleRepository GetRoleRepository()
        {
            return RoleRepository
                ?? (RoleRepository = new RoleRepository(_context, _loggerRoleRepository));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _context?.Dispose();

            _context = null;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_context != null)
                await _context.DisposeAsync();

            _context = null;
        }
    }
}
