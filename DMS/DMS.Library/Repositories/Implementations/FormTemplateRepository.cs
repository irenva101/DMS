using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class FormTemplateRepository : Repository<FormTemplateEntity>, IFormTemplateRepository
    {
        public FormTemplateRepository(DmsContext context, ILogger<FormTemplateRepository> logger)
            : base(context, logger) { }
    }
}
