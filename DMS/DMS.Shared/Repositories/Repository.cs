using CSharpFunctionalExtensions;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DMS.Shared.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly DbContext _dbContext;
        protected readonly ILogger<Repository<TEntity>> _logger;

        public Repository(DbContext dbContext, ILogger<Repository<TEntity>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        private static void HandleRepositoryOptions<T>(ref RepositoryOptions? options, ref IQueryable<T> query) where T : TEntity
        {
            options ??= new();

            if (options.AsNoTracking)
                query = query.AsNoTracking();

            if (options.IsSplitQuery)
                query = query.AsSplitQuery();

            if (options.IgnoreQueryFilters)
                query = query.IgnoreQueryFilters();
        }

        private IQueryable<TChild> Query<TChild>() where TChild : TEntity
        {
            return _dbContext.Set<TEntity>()
                .OfType<TChild>();
        }

        public virtual async Task<Result<EntityEntry<TEntity>, ResponseError>> AddAsync(
            TEntity entity
        )
        {
            try
            {
                return await _dbContext.Set<TEntity>().AddAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError()
                {
                    ErrorCode = ResponseErrorCode.Exception,
                    Message = "An error occurred.",
                };
            }
        }

        public virtual Result<EntityEntry<TEntity>, ResponseError> Update(
              TEntity entity
          )
        {
            try
            {
                return _dbContext.Set<TEntity>().Update(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError()
                {
                    ErrorCode = ResponseErrorCode.Exception,
                    Message = "An error occurred.",
                };
            }
        }

        public async Task<Result<bool, ResponseError>> AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await _dbContext.Set<TEntity>().AddRangeAsync(entities);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError()
                {
                    ErrorCode = ResponseErrorCode.Exception,
                    Message = "An error occurred.",
                };
            }
        }

        public async Task<Result<IEnumerable<TEntity>, ResponseError>> FindAsync(
            Expression<Func<TEntity, bool>> predicate
        )
        {
            try
            {
                var entityFromDb = await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();

                if (entityFromDb != null)
                {
                    return entityFromDb;
                }
                else
                {
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The entity doesn't exist in the system."
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<List<long?>, ResponseError>> FindIdsAsync(
            Expression<Func<TEntity, bool>> predicate
        )
        {
            try
            {
                var ids = await _dbContext.Set<TEntity>()
                    .Where(predicate)
                    .Select(x => (long?)x.Id)
                    .ToListAsync();
                return ids;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<int, ResponseError>> GetCountAsync()
        {
            try
            {
                return await _dbContext.Set<TEntity>().Where(x => !x.IsDeleted).CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        /// <summary>
        /// Retrieves a specified entity with included linked entities.
        /// WARNING: The main entity will be ignored if the "IsDeleted" property is set, but in the included entity,
        /// this will not be checked. Make sure the included entity uses a filter in the Context file as it was done
        /// for MeterSnapshotEntity and for DlmsMeterModelConfigurationEntity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public async Task<Result<TEntity, ResponseError>> GetByIdAsync(
            long? id,
            params Expression<Func<TEntity, object>>[] includes
        )
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                var entityFromDb = await query.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
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

        public async Task<Result<TEntity, ResponseError>> GetByDmsIdAsync(
            Guid? id,
            params Expression<Func<TEntity, object>>[] includes
        )
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();
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
        public async Task<Result<TEntity, ResponseError>> GetByIdMultiLevelAsync(
            long? id,
            params Tuple<
                Expression<Func<TEntity, object>>,
                Expression<Func<object, object>>
            >[] includes
        )
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();
                foreach (var item in includes)
                {
                    if (item.Item2 != null)
                    {
                        query = query.Include(item.Item1).ThenInclude(item.Item2);
                    }
                    else
                    {
                        query = query.Include(item.Item1);
                    }
                }

                var entityFromDb = await query.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The entity doesn't exist in the system."
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

        public async Task<Result<TEntity, ResponseError>> GetByDmsIdMultiLevelAsync(
            Guid id,
            params Tuple<
                Expression<Func<TEntity, object>>,
                Expression<Func<object, object>>
            >[] includes
        )
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                foreach (var item in includes)
                {
                    if (item.Item2 != null)
                    {
                        query = query.Include(item.Item1).ThenInclude(item.Item2); // Warning: Can't safely do ThenInclude
                    }
                    else
                    {
                        query = query.Include(item.Item1);
                    }
                }

                var entityFromDb = await query.FirstOrDefaultAsync(x =>
                    !x.IsDeleted && x.DmsId == id
                );
                if (entityFromDb == null)
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The entity doesn't exist in the system."
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

        public Result<bool, ResponseError> Remove(TEntity entity)
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

        public Result<bool, ResponseError> RemoveRange(IEnumerable<TEntity> entities)
        {
            try
            {
                foreach (var entity in entities)
                {
                    entity.LastUpdateTime = DateTime.UtcNow;
                    entity.IsDeleted = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<bool, ResponseError>> AnyAsync(
            Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                var exists = await query.AnyAsync(predicate);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<TEntity, ResponseError>> FindSingleAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                var result = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (result == null)
                    return new ResponseError(ResponseErrorCode.Database_NotFound, "The entity doesn't exist in the system.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<TChild, ResponseError>> FindSingleAsync<TChild>(
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = null,
            CancellationToken cancellationToken = default) where TChild : TEntity
        {
            try
            {
                var query = Query<TChild>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                var result = await query.FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (result == null)
                    return new ResponseError(ResponseErrorCode.Database_NotFound, "The entity doesn't exist in the system.");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<PaginationDataOut<TEntity>, ResponseError>> FindPageAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                return new PaginationDataOut<TEntity>
                {
                    Count = await query.CountAsync(cancellationToken: cancellationToken),
                    Data = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken: cancellationToken)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<PaginationDataOut<TChild>, ResponseError>> FindPageAsync<TChild>(
            int page,
            int pageSize,
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = null,
            CancellationToken cancellationToken = default) where TChild : TEntity
        {
            try
            {
                var query = Query<TChild>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                return new PaginationDataOut<TChild>
                {
                    Count = await query.CountAsync(cancellationToken: cancellationToken),
                    Data = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync(cancellationToken: cancellationToken)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<List<TEntity>, ResponseError>> FindAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext
                .Set<TEntity>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                return await query.ToListAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<List<TChild>, ResponseError>> FindAsync<TChild>(
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = null,
            CancellationToken cancellationToken = default) where TChild : TEntity
        {
            try
            {
                var query = Query<TChild>();

                HandleRepositoryOptions(ref options, ref query);

                if (filter != null)
                    query = query.Where(filter);

                if (include != null)
                    query = include(query);

                query = query.OrderBy(x => x.Id);

                if (orderBy != null)
                    query = orderBy(query)
                        .ThenBy(x => x.Id);

                return await query.ToListAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public Result<bool, ResponseError> UpdateRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _dbContext.Set<TEntity>().UpdateRange(entities);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError()
                {
                    ErrorCode = ResponseErrorCode.Exception,
                    Message = "An error occurred.",
                };
            }
        }
    }
}
