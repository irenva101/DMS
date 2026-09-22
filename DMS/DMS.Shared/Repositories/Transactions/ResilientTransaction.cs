using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DMS.Shared.Repositories.Transactions
{
    public class ResilientTransaction : IResilientTransaction
    {
        private readonly DbContext _context;

        private ResilientTransaction(DbContext context) =>
            _context = context ?? throw new ArgumentNullException(nameof(context));

        public static ResilientTransaction New(DbContext context) =>
            new(context);

        public async Task<T> ExecuteAsync<T>(
            Func<Task<T>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default) where T : IResult
        {
            if (_context.Database.CurrentTransaction != null)
                return await action();

            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

                try
                {
                    var result = await action();

                    if (result.IsSuccess)
                    {
                        await _context.SaveChangesAsync(cancellationToken);
                        await transaction.CommitAsync(cancellationToken);
                    }
                    else
                    {
                        await transaction.RollbackAsync(cancellationToken);
                    }

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}
