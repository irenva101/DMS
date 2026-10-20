using CSharpFunctionalExtensions;
using DMS.Shared.Commons;

namespace DMS.Library.Repositories.Interfaces
{
    // Not an IRepository<T>: CodeSequenceEntity is keyed by (prefix, year) and does not inherit Entity
    public interface ICodeSequenceRepository
    {
        // Must run in the same transaction that creates the record, so a rollback leaves no gap
        Task<Result<int, ResponseError>> GetNextValue(string prefix, int year);
    }
}
