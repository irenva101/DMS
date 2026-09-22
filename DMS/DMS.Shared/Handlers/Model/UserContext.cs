namespace DMS.Shared.Handlers.Model
{
    public class UserContext : IDisposable, IAsyncDisposable
    {
        public string UtilityName { get; set; }
        public Guid UtilityId { get; set; }
        public int TimeZone { get; set; }

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
            UtilityName = string.Empty;
            UtilityId = Guid.Empty;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            UtilityName = string.Empty;
            UtilityId = Guid.Empty;
        }
    }
}
