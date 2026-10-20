using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class AuditLogRepository : Repository<AuditLogEntity>, IAuditLogRepository
    {
        public AuditLogRepository(DmsContext context, ILogger<AuditLogRepository> logger)
            : base(context, logger) { }
    }
}
