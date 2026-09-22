using DMS.Shared.Handlers.Interfaces;
using DMS.Shared.Handlers.Model;

namespace DMS.Shared.Handlers.Implementations
{
    public class UserContextHandler : IUserContextHandler
    {
        protected UserContext _userContext;

        public UserContextHandler(UserContext userContext)
        {
            _userContext = userContext;
        }

        public void SetUtilityInContext(string prefix, Guid id, int timeZone = 0)
        {
            _userContext.UtilityName = prefix;
            _userContext.UtilityId = id;
            _userContext.TimeZone = timeZone;
        }

        public void SetSystemUtilityContext(string prefix, Guid id)
        {
            _userContext.UtilityName = prefix;
            _userContext.UtilityId = id;
        }

        public UserContext GetUtilityFromContext()
        {
            return _userContext;
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _userContext?.Dispose();

            _userContext = null;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_userContext != null)
                await _userContext.DisposeAsync();

            _userContext = null;
        }
    }
}
