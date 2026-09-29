using System.Reflection;

namespace DMS.API.Services.GitRevisionService
{
    public class GitRevisionService : IGitRevisionService
    {
        private readonly string? _commitHash;

        public GitRevisionService(ILogger<GitRevisionService> logger)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var info = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (info == null)
            {
                _commitHash = null;
                return;
            }
            var parts = info.Split('+');
            _commitHash = parts.Length > 1 ? parts[1] : info;
        }

        public string? GetCommitHash() => _commitHash;
    }
}
