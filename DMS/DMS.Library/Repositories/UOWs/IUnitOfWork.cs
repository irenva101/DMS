using CSharpFunctionalExtensions;
using DMS.Shared.Commons;
using DMS.Shared.Repositories.Transactions;

namespace DMS.Library.Repositories.UOWs
{
    public interface IUnitOfWork
    {
        public Task<Result<int, ResponseError>> CompleteAsync();
        public Task<Result<bool, ResponseError>> MigrateDb();
        IResilientTransaction CreateResilientTransaction();
    }
}
