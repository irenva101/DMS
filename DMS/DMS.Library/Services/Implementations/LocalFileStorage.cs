using CSharpFunctionalExtensions;
using DMS.Library.Services.Interfaces;
using DMS.Shared.Commons;
using DMS.Shared.Config;
using DMS.Shared.Handlers.Model;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Services.Implementations
{
    public class LocalFileStorage(DmsConfig _config, UserContext _userContext, ILogger<LocalFileStorage> _logger) : IFileStorage
    {
        private string FullPath(string key)
        {
            // Without login there is no tenant on the request, same as the "default" database fallback
            var tenant = string.IsNullOrEmpty(_userContext.UtilityName) ? "DEFAULT" : _userContext.UtilityName;
            return Path.Combine(_config.FileStorage.RootPath, tenant, key);
        }

        public async Task<Result<bool, ResponseError>> SaveAsync(string key, Stream content)
        {
            try
            {
                var path = FullPath(key);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                await using var file = new FileStream(path, FileMode.CreateNew);
                await content.CopyToAsync(file);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public Result<Stream, ResponseError> OpenRead(string key)
        {
            try
            {
                return new FileStream(FullPath(key), FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
            }
            catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException)
            {
                _logger.LogError(ex, "File missing on disk for storage key {StorageKey}", key);
                return new ResponseError(ResponseErrorCode.Database_NotFound, "The file doesn't exist in the system.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }
    }
}
