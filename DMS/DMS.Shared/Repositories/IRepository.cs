using CSharpFunctionalExtensions;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace DMS.Shared.Repositories
{
    public record RepositoryOptions(
        bool AsNoTracking = false,
        bool IsSplitQuery = false,
        bool IgnoreQueryFilters = false);

    public interface IRepository<TEntity>
        where TEntity : class
    {
        Task<Result<TEntity, ResponseError>> GetByIdAsync(
            long? id,
            params Expression<Func<TEntity, object>>[] includes
        );
        Task<Result<TEntity, ResponseError>> GetByDmsIdAsync(
            Guid? id,
            params Expression<Func<TEntity, object>>[] includes
        );

        Task<Result<TEntity, ResponseError>> GetByIdMultiLevelAsync(
            long? id,
            params Tuple<
                Expression<Func<TEntity, object>>,
                Expression<Func<object, object>>
            >[] includes
        );

        Task<Result<TEntity, ResponseError>> GetByDmsIdMultiLevelAsync(
            Guid id,
            params Tuple<
                Expression<Func<TEntity, object>>,
                Expression<Func<object, object>>
            >[] includes
        );

        Task<Result<int, ResponseError>> GetCountAsync();

        Task<Result<IEnumerable<TEntity>, ResponseError>> FindAsync(
            Expression<Func<TEntity, bool>> predicate
        );

        Task<Result<EntityEntry<TEntity>, ResponseError>> AddAsync(TEntity entity);

        Result<EntityEntry<TEntity>, ResponseError> Update(TEntity entity);

        Result<bool, ResponseError> UpdateRange(IEnumerable<TEntity> entities);

        Task<Result<bool, ResponseError>> AddRangeAsync(IEnumerable<TEntity> entities);

        Result<bool, ResponseError> Remove(TEntity entity);

        Result<bool, ResponseError> RemoveRange(IEnumerable<TEntity> entities);

        Task<Result<bool, ResponseError>> AnyAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = false);

        Task<Result<TEntity, ResponseError>> FindSingleAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default);

        Task<Result<TChild, ResponseError>> FindSingleAsync<TChild>(
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default) where TChild : TEntity;

        Task<Result<PaginationDataOut<TEntity>, ResponseError>> FindPageAsync(
            int page,
            int pageSize,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default);

        Task<Result<PaginationDataOut<TChild>, ResponseError>> FindPageAsync<TChild>(
            int page,
            int pageSize,
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default) where TChild : TEntity;

        Task<Result<List<TEntity>, ResponseError>> FindAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default);

        Task<Result<List<TChild>, ResponseError>> FindAsync<TChild>(
            Expression<Func<TChild, bool>>? filter = null,
            Func<IQueryable<TChild>, IIncludableQueryable<TChild, object>>? include = null,
            Func<IQueryable<TChild>, IOrderedQueryable<TChild>>? orderBy = null,
            RepositoryOptions? options = default,
            CancellationToken cancellationToken = default) where TChild : TEntity;

        Task<Result<List<long?>, ResponseError>> FindIdsAsync(
            Expression<Func<TEntity, bool>> predicate
        );
    }

}
