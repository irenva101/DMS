using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class DocumentRepository : Repository<DocumentEntity>, IDocumentRepository
    {
        public DocumentRepository(DmsContext context, ILogger<DocumentRepository> logger)
            : base(context, logger) { }
    }
}
