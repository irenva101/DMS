using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class StepDefinitionRepository : Repository<StepDefinitionEntity>, IStepDefinitionRepository
    {
        public StepDefinitionRepository(DmsContext context, ILogger<StepDefinitionRepository> logger)
            : base(context, logger) { }
    }
}
