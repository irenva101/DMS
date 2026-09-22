using DMS.Shared.Handlers.Model;

namespace DMS.Shared.Handlers.Interfaces
{
    public interface IUserContextHandler : IDisposable, IAsyncDisposable
    {
        void SetUtilityInContext(string prefix, Guid id, int timeZone = 0);

        void SetSystemUtilityContext(string prefix, Guid id);


        UserContext GetUtilityFromContext();
    }
}
