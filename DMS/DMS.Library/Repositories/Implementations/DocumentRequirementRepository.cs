using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class DocumentRequirementRepository : Repository<DocumentRequirementEntity>, IDocumentRequirementRepository
    {
        public DocumentRequirementRepository(DmsContext context, ILogger<DocumentRequirementRepository> logger)
            : base(context, logger) { }
    }
}
