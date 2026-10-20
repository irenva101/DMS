using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class StepRepository : Repository<StepEntity>, IStepRepository
    {
        public StepRepository(DmsContext context, ILogger<StepRepository> logger)
            : base(context, logger) { }
    }
}
