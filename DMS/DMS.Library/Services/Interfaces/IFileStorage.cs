using CSharpFunctionalExtensions;
using DMS.Shared.Commons;

namespace DMS.Library.Services.Interfaces
{
    // The only place that touches the disk, so moving to blob storage later means replacing one class
    public interface IFileStorage
    {
        // Key is relative to the tenant folder and stored as documents.storage_key
        Task<Result<bool, ResponseError>> SaveAsync(string key, Stream content);
        Result<Stream, ResponseError> OpenRead(string key);
    }
}
