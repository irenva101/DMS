using CSharpFunctionalExtensions;
using DMS.Library.Repositories.Interfaces;
using DMS.Shared.Commons;
using DMS.Shared.Repositories.Transactions;

namespace DMS.Library.Repositories.UOWs
{
    public interface IUnitOfWork
    {
        public Task<Result<int, ResponseError>> CompleteAsync();
        public Task<Result<bool, ResponseError>> MigrateDb();
        IResilientTransaction CreateResilientTransaction();
        public IAuditLogRepository GetAuditLogRepository();
        public IBillRepository GetBillRepository();
        public ICodeSequenceRepository GetCodeSequenceRepository();
        public ICompanyRepository GetCompanyRepository();
        public IContractRepository GetContractRepository();
        public IDocumentRepository GetDocumentRepository();
        public IDocumentRequirementRepository GetDocumentRequirementRepository();
        public IFormTemplateRepository GetFormTemplateRepository();
        public IProcessRepository GetProcessRepository();
        public IProcessTypeRepository GetProcessTypeRepository();
        public IStepRepository GetStepRepository();
        public IStepDefinitionRepository GetStepDefinitionRepository();
    }
}
