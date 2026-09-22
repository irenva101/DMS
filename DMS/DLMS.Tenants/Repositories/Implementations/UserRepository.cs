using CSharpFunctionalExtensions;
using DLMS.Tenants.Repositories.Interfaces;
using DLMS.Tenants.Repositories.Models;
using DLMS.Tenants.Services.Models.User.DataIn;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace DLMS.Tenants.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        protected readonly DbContext _dbContext;
        protected readonly ILogger<UserRepository> _logger;

        public UserRepository(DbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<UserEntity, ResponseError>> GetByEmailAllUtilities(string email)
        {
            try
            {
                var user = await _dbContext
                    .Set<UserEntity>()
                    .Include(x => x.Utility)
                    .Include(x => x.Role)
                    .Include(x => x.CreatedBy)
                    .FirstOrDefaultAsync(x => !x.IsDeleted && x.Email.ToUpper() == email.ToUpper());
                if (user == null)
                {
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "The given user doesn't exist in the system."
                    );
                }
                else
                {
                    return user;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<List<UserEntity>, ResponseError>> GetByGuids(List<Guid> dmsIds, Guid utilityId)
        {
            try
            {
                var users = await _dbContext
                    .Set<UserEntity>()
                    .Include(x => x.CreatedBy)
                    .Include(x => x.Utility)
                    .Include(x => x.Role)
                    .AsNoTracking()
                    .Where(x => !x.IsDeleted && dmsIds.Contains(x.DmsId) && x.Utility.DmsId == utilityId)
                    .ToListAsync();

                if (users == null || users.Count == 0)
                {
                    return new ResponseError(
                        ResponseErrorCode.Database_NotFound,
                        "None of the given users exist in the system."
                    );
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred while retrieving users.");
            }
        }


        public async Task<Result<List<PermissionEntity>, ResponseError>> GetPermissions(Guid dmsId, Guid utilityId)
        {
            try
            {
                var user = await _dbContext
                    .Set<UserEntity>()
                    .Include(x => x.Role) // Important: Include the Role
                        .ThenInclude(r => r.Permissions.Where(p => !p.IsDeleted)) // And the Permissions
                    .Where(x => !x.IsDeleted && x.DmsId == dmsId && x.Utility.DmsId == utilityId)
                    .FirstOrDefaultAsync();

                return user?.Role?.Permissions ?? [];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        private IQueryable<UserEntity> GetPagingQuery()
        {
            var query = _dbContext
                .Set<UserEntity>()
                .Include(x => x.CreatedBy)
                .Include(x => x.Utility)
                .Include(x => x.Role)
                .ThenInclude(x => x.Permissions)
                .Where(x => !x.IsDeleted);
            return query;
        }

        public async Task<Result<PaginationDataOut<UserEntity>, ResponseError>> Query(
            UserGetAllDataIn dataIn, Guid utilityGuid
        )
        {
            try
            {
                var query = GetPagingQuery();
                //utility filter
                query = query.Where(x => utilityGuid == x.Utility.DmsId);

                //search by name
                if (!string.IsNullOrWhiteSpace(dataIn.Search))
                    query = query.Where(x =>
                        x.FirstName.ToUpper().Contains(dataIn.Search.ToUpper())
                        || x.LastName.ToUpper().Contains(dataIn.Search.ToUpper())
                        || x.Email.ToUpper().Contains(dataIn.Search.ToUpper())
                        || x.MiddleName.ToUpper().Contains(dataIn.Search.ToUpper())
                    );

                //sorting
                if (dataIn.Sorting == SortingType.Ascending)
                    query = query.OrderBy(x => x.LastUpdateTime);
                else if (dataIn.Sorting == SortingType.Descending)
                    query = query.OrderByDescending(x => x.LastUpdateTime);
                else
                    query = query.OrderByDescending(x => x.Id);

                return new PaginationDataOut<UserEntity>()
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

        public async Task<Result<Dictionary<string, UserEntity>, ResponseError>> GetAllDictionary(Guid utilityId)
        {
            try
            {
                var query = _dbContext.Set<UserEntity>().Where(x => !x.IsDeleted && x.Utility.DmsId == utilityId);
                return await query.ToDictionaryAsync(x => x.Email, x => x);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public async Task<Result<string, ResponseError>> SaveRangeExtension(
            List<UserEntity> addUsers,
            List<UserEntity> updateUsers
        )
        {
            try
            {
                var result = new Result<string, ResponseError>();
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

                var executionStrategy = _dbContext.Database.CreateExecutionStrategy();

                await executionStrategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        _dbContext.Set<UserEntity>().AddRange(addUsers);
                        _dbContext.Set<UserEntity>().UpdateRange(updateUsers);
                        await _dbContext.SaveChangesAsync();

                        await transaction.CommitAsync();
                        result = "Success saving.";
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                        result = new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
                    }
                });
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
            finally
            {
                _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            }
        }

        public async Task<Result<UserEntity, ResponseError>> GetByDmsIdAsync(Guid dmsId, Guid utilityId, params Expression<Func<UserEntity, object>>[] includes)
        {
            try
            {
                IQueryable<UserEntity> query = _dbContext.Set<UserEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                var entityFromDb = await query.FirstOrDefaultAsync(x =>
                    !x.IsDeleted && x.DmsId == dmsId && x.Utility.DmsId == utilityId
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

        public async Task<Result<UserEntity, ResponseError>> AddAsync(UserEntity user)
        {
            try
            {
                var entity = await _dbContext.Set<UserEntity>().AddAsync(user);
                return entity.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public Result<bool, ResponseError> Remove(UserEntity entity)
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

        public async Task<Result<UserEntity, ResponseError>> GetByDmsIdAllUtilitiesAsync(Guid dmsId, params Expression<Func<UserEntity, object>>[] includes)
        {
            try
            {
                IQueryable<UserEntity> query = _dbContext.Set<UserEntity>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                var entityFromDb = await query.FirstOrDefaultAsync(x =>
                    !x.IsDeleted && x.DmsId == dmsId
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
    }
}
