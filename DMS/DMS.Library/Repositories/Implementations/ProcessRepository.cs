using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class ProcessRepository : Repository<ProcessEntity>, IProcessRepository
    {
        public ProcessRepository(DmsContext context, ILogger<ProcessRepository> logger)
            : base(context, logger) { }
    }
}
