using CSharpFunctionalExtensions;
using System.Data;

namespace DMS.Shared.Repositories.Transactions
{
    public interface IResilientTransaction
    {
        Task<T> ExecuteAsync<T>(
            Func<Task<T>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default) where T : IResult;
    }
}
