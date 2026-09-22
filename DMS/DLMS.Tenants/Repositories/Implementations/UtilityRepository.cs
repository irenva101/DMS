using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Interfaces;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.Utility.DataIn;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Implementations
{
    public class UtilityRepository : IUtilityRepository
    {
        protected readonly DbContext _dbContext;
        protected readonly ILogger<UtilityRepository> _logger;

        public UtilityRepository(DbContext dbContext, ILogger<UtilityRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<List<UtilityEntity>, ResponseError>> GetAll()
        {
            try
            {
                return await _dbContext
                    .Set<UtilityEntity>()
                    .Where(x => !x.IsDeleted).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        private IQueryable<UtilityEntity> GetPagingQuery()
        {
            var query = _dbContext
                .Set<UtilityEntity>()
                .Include(x => x.ContactPerson)
                .Where(x => !x.IsDeleted);
            return query;
        }

        public async Task<Result<PaginationDataOut<UtilityEntity>, ResponseError>> Query(
            UtilityGetAllDataIn dataIn
        )
        {
            try
            {
                var query = GetPagingQuery();

                //search by name
                if (!string.IsNullOrWhiteSpace(dataIn.Search))
                    query = query.Where(x =>
                        x.Name.ToUpper().Contains(dataIn.Search.ToUpper())
                        || x.Acronym.ToUpper().Contains(dataIn.Search.ToUpper())
                    );

                //filter by date
                if (dataIn.StartDate != null)
                    query = query.Where(x => x.CreationDate >= dataIn.StartDate.Value);

                if (dataIn.EndDate != null)
                    query = query.Where(x => x.CreationDate <= dataIn.EndDate.Value);

                if (dataIn.Status != null)
                    query = query.Where(x => x.Status == dataIn.Status);

                //sorting
                if (dataIn.Sorting == SortingType.Ascending)
                    query = query.OrderBy(x => x.LastUpdateTime);
                else if (dataIn.Sorting == SortingType.Descending)
                    query = query.OrderByDescending(x => x.LastUpdateTime);
                else
                    query = query.OrderByDescending(x => x.Id);

                return new PaginationDataOut<UtilityEntity>()
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

        public async Task<Result<UtilityEntity, ResponseError>> GetByAcronym(string data)
        {
            try
            {
                var query = _dbContext
                    .Set<UtilityEntity>()
                    .Where(x => !x.IsDeleted);


                var entityFromDb = await query.FirstOrDefaultAsync(x => !x.IsDeleted && x.Acronym.ToLower() == data.ToLower());
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given utility doesn't exist in the system."
                    );
                else
                    return entityFromDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<UtilityEntity, ResponseError>> GetByName(string data)
        {
            try
            {
                var query = _dbContext
                    .Set<UtilityEntity>()
                    .Where(x => !x.IsDeleted);


                var entityFromDb = await query.FirstOrDefaultAsync(x => !x.IsDeleted && x.Name.ToLower() == data.ToLower());
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given utility doesn't exist in the system."
                    );
                else
                    return entityFromDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<UtilityEntity, ResponseError>> GetByApiKey(Guid apikey)
        {
            try
            {
                var query = _dbContext
                    .Set<UtilityEntity>()
                    .Where(x => !x.IsDeleted);


                var entityFromDb = await query.FirstOrDefaultAsync(x => x.ApiKey == apikey);
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given utility doesn't exist in the system."
                    );
                else
                    return entityFromDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<UtilityEntity, ResponseError>> GetByDmsIdAsync(Guid id, params Expression<Func<UtilityEntity, object>>[] includes)
        {
            try
            {
                IQueryable<UtilityEntity> query = _dbContext.Set<UtilityEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                var entityFromDb = await query.FirstOrDefaultAsync(x =>
                    !x.IsDeleted && x.DmsId == id
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

        public async Task<Result<UtilityEntity, ResponseError>> AddAsync(UtilityEntity utility)
        {
            try
            {
                var entity = await _dbContext.Set<UtilityEntity>().AddAsync(utility);
                return entity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public Result<bool, ResponseError> Remove(UtilityEntity entity)
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
