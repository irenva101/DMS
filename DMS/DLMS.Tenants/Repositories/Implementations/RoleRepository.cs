using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Interfaces;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Role.DataIn;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        protected readonly DbContext _dbContext;
        protected readonly ILogger<RoleRepository> _logger;

        public RoleRepository(DbContext dbContext, ILogger<RoleRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<RoleEntity, ResponseError>> GetByName(string roleName, Guid utilityId)
        {
            try
            {
                var role = await _dbContext
                    .Set<RoleEntity>()
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.Name == roleName && x.Utility.DmsId == utilityId);
                if (role == null)
                {
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given role doesn't exist in the system."
                    );
                }
                else
                {
                    return role;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        private IQueryable<RoleEntity> GetPagingQuery(Guid utilityId)
        {
            var query = _dbContext
                .Set<RoleEntity>()
                .Include(x => x.Permissions)
                .Where(x => !x.IsDeleted && x.Utility.DmsId == utilityId);
            return query;
        }

        public async Task<Result<PaginationDataOut<RoleEntity>, ResponseError>> Query(
            RoleGetAllDataIn dataIn, Guid utilityId
        )
        {
            try
            {
                var query = GetPagingQuery(utilityId);

                query = query.Where(x => x.Name.ToLower().Trim() != "superadmin");

                //search by name
                if (!string.IsNullOrWhiteSpace(dataIn.Search))
                    query = query.Where(x => x.Name.ToUpper().Contains(dataIn.Search.ToUpper()));

                //sorting
                if (dataIn.Sorting == SortingType.Ascending)
                    query = query.OrderBy(x => x.LastUpdateTime);
                else if (dataIn.Sorting == SortingType.Descending)
                    query = query.OrderByDescending(x => x.LastUpdateTime);
                else
                    query = query.OrderByDescending(x => x.Id);

                var dataResult = await query
                    .Skip((dataIn.PageInfo.Page - 1) * dataIn.PageInfo.PageSize)
                    .Take(dataIn.PageInfo.PageSize)
                    .AsNoTracking()
                    .ToListAsync();

                return new PaginationDataOut<RoleEntity>()
                {
                    Count = await query.CountAsync(),
                    Data = dataResult,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<IEnumerable<RoleEntity>, ResponseError>> GetAllAsync(Guid utilityId,
            params Expression<Func<RoleEntity, object>>[] includes
        )
        {
            try
            {
                IQueryable<RoleEntity> query = _dbContext.Set<RoleEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                return await query.Where(x => !x.IsDeleted && x.Utility.DmsId == utilityId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<Dictionary<string, RoleEntity>, ResponseError>> GetAllDictionary(Guid utilityId)
        {
            try
            {
                var query = _dbContext.Set<RoleEntity>().Where(x => !x.IsDeleted && x.Utility.DmsId == utilityId);
                return await query.ToDictionaryAsync(x => x.Name, x => x);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<RoleEntity, ResponseError>> GetByDmsIdAsync(Guid id, Guid utilityId, params Expression<Func<RoleEntity, object>>[] includes)
        {
            try
            {
                IQueryable<RoleEntity> query = _dbContext.Set<RoleEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                var entityFromDb = await query.FirstOrDefaultAsync(x =>
                    !x.IsDeleted && x.DmsId == id && x.Utility.DmsId == utilityId
                );
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given entity doesn't exist in the system."
                    );
                else
                    return entityFromDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<RoleEntity, ResponseError>> AddAsync(RoleEntity role)
        {
            try
            {
                var entity = await _dbContext.Set<RoleEntity>().AddAsync(role);
                return entity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public Result<bool, ResponseError> Remove(RoleEntity entity)
        {
            try
            {
                entity.LastUpdateTime = DateTime.UtcNow;
                entity.IsDeleted = true;
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }
    }
}
