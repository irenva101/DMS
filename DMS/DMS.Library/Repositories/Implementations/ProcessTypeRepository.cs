using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class ProcessTypeRepository : Repository<ProcessTypeEntity>, IProcessTypeRepository
    {
        public ProcessTypeRepository(DmsContext context, ILogger<ProcessTypeRepository> logger)
            : base(context, logger) { }
    }
}
