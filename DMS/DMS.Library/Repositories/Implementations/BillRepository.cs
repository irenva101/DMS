using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class BillRepository : Repository<BillEntity>, IBillRepository
    {
        public BillRepository(DmsContext context, ILogger<BillRepository> logger)
            : base(context, logger) { }
    }
}
