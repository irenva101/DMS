using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Library.Repositories.Models;
using DMS.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class CompanyRepository : Repository<CompanyEntity>, ICompanyRepository
    {
        public CompanyRepository(DmsContext context, ILogger<CompanyRepository> logger)
            : base(context, logger) { }
    }
}
