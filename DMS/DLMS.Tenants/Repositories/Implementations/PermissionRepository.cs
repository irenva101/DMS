using CSharpFunctionalExtensions;
using DLMS.Tenants.Data;
using DLMS.Tenants.Repositories.Interfaces;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Permissions.DataIn;
using DMS.Shared.Commons;
using DMS.Shared.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DLMS.Tenants.Repositories.Implementations
{
    public class PermissionRepository : Repository<PermissionEntity>, IPermissionRepository
    {
        public SharedContext IdDatabaseContext
        {
            get { return _dbContext as SharedContext; }
        }

        public PermissionRepository(SharedContext context, ILogger<PermissionRepository> logger)
            : base(context, logger) { }

        public async Task<Result<List<PermissionEntity>, ResponseError>> GetAllAvailablePermissions()
        {
            try
            {
                return await _dbContext.Set<PermissionEntity>().Where(x => !x.IsDeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<List<PermissionEntity>, ResponseError>> GetPermissionsForListValues(
            List<PermissionsEnum> perms
        )
        {
            try
            {
                var allPermissions = await _dbContext
                    .Set<PermissionEntity>()
                    .Where(x => !x.IsDeleted && perms.Contains(x.Value))
                    .ToListAsync();

                if (allPermissions.Count != perms.Count)
                {
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "Some of the permissions do not exist in the database."
                    );
                }
                return allPermissions.Where(permission => perms.Contains(permission.Value)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        private IQueryable<PermissionEntity> GetPagingQuery()
        {
            var query = _dbContext
                .Set<PermissionEntity>()
                .Include(x => x.Roles)
                .Where(x => !x.IsDeleted);
            return query;
        }

        public async Task<Result<PaginationDataOut<PermissionEntity>, ResponseError>> Query(
            PermissionGetAllDataIn dataIn
        )
        {
            try
            {
                var query = GetPagingQuery();

                //sorting
                if (dataIn.Sorting == SortingType.Ascending)
                    query = query.OrderBy(x => x.LastUpdateTime);
                else if (dataIn.Sorting == SortingType.Descending)
                    query = query.OrderByDescending(x => x.LastUpdateTime);
                else
                    query = query.OrderByDescending(x => x.Id);

                return new PaginationDataOut<PermissionEntity>()
                {
                    Count = await query.CountAsync(),
                    Data = await query
                        .Skip((dataIn.PageInfo.Page - 1) * dataIn.PageInfo.PageSize)
                        .Take(dataIn.PageInfo.PageSize)
                        .AsNoTracking()
                        .ToListAsync(),
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }
    }
}
