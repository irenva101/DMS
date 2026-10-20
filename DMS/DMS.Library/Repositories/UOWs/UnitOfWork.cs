using CSharpFunctionalExtensions;
using DMS.Library.Data;
using DMS.Library.Repositories.Implementations;
using DMS.Library.Repositories.Interfaces;
using DMS.Shared.Commons;
using DMS.Shared.Repositories.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.UOWs
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DmsContext _context;
        private readonly ILogger<UnitOfWork> _logger;
        private readonly ILoggerFactory _loggerFactory;

        public UnitOfWork(
            DmsContext context,
            ILoggerFactory loggerFactory
        )
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<UnitOfWork>();
        }

        private IAuditLogRepository AuditLogRepository { get; set; }
        private IBillRepository BillRepository { get; set; }
        private ICodeSequenceRepository CodeSequenceRepository { get; set; }
        private ICompanyRepository CompanyRepository { get; set; }
        private IContractRepository ContractRepository { get; set; }
        private IDocumentRepository DocumentRepository { get; set; }
        private IDocumentRequirementRepository DocumentRequirementRepository { get; set; }
        private IFormTemplateRepository FormTemplateRepository { get; set; }
        private IProcessRepository ProcessRepository { get; set; }
        private IProcessTypeRepository ProcessTypeRepository { get; set; }
        private IStepRepository StepRepository { get; set; }
        private IStepDefinitionRepository StepDefinitionRepository { get; set; }

        public async Task<Result<int, ResponseError>> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public IResilientTransaction CreateResilientTransaction()
        {
            return ResilientTransaction.New(_context);
        }

        public async Task<Result<bool, ResponseError>> MigrateDb()
        {
            try
            {
                await _context.Database.MigrateAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error: {ExceptionMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }

        public IAuditLogRepository GetAuditLogRepository()
        {
            return AuditLogRepository
                ?? (AuditLogRepository = new AuditLogRepository(_context, _loggerFactory.CreateLogger<AuditLogRepository>()));
        }

        public IBillRepository GetBillRepository()
        {
            return BillRepository
                ?? (BillRepository = new BillRepository(_context, _loggerFactory.CreateLogger<BillRepository>()));
        }

        public ICodeSequenceRepository GetCodeSequenceRepository()
        {
            return CodeSequenceRepository
                ?? (CodeSequenceRepository = new CodeSequenceRepository(_context, _loggerFactory.CreateLogger<CodeSequenceRepository>()));
        }

        public ICompanyRepository GetCompanyRepository()
        {
            return CompanyRepository
                ?? (CompanyRepository = new CompanyRepository(_context, _loggerFactory.CreateLogger<CompanyRepository>()));
        }

        public IContractRepository GetContractRepository()
        {
            return ContractRepository
                ?? (ContractRepository = new ContractRepository(_context, _loggerFactory.CreateLogger<ContractRepository>()));
        }

        public IDocumentRepository GetDocumentRepository()
        {
            return DocumentRepository
                ?? (DocumentRepository = new DocumentRepository(_context, _loggerFactory.CreateLogger<DocumentRepository>()));
        }

        public IDocumentRequirementRepository GetDocumentRequirementRepository()
        {
            return DocumentRequirementRepository
                ?? (DocumentRequirementRepository = new DocumentRequirementRepository(_context, _loggerFactory.CreateLogger<DocumentRequirementRepository>()));
        }

        public IFormTemplateRepository GetFormTemplateRepository()
        {
            return FormTemplateRepository
                ?? (FormTemplateRepository = new FormTemplateRepository(_context, _loggerFactory.CreateLogger<FormTemplateRepository>()));
        }

        public IProcessRepository GetProcessRepository()
        {
            return ProcessRepository
                ?? (ProcessRepository = new ProcessRepository(_context, _loggerFactory.CreateLogger<ProcessRepository>()));
        }

        public IProcessTypeRepository GetProcessTypeRepository()
        {
            return ProcessTypeRepository
                ?? (ProcessTypeRepository = new ProcessTypeRepository(_context, _loggerFactory.CreateLogger<ProcessTypeRepository>()));
        }

        public IStepRepository GetStepRepository()
        {
            return StepRepository
                ?? (StepRepository = new StepRepository(_context, _loggerFactory.CreateLogger<StepRepository>()));
        }

        public IStepDefinitionRepository GetStepDefinitionRepository()
        {
            return StepDefinitionRepository
                ?? (StepDefinitionRepository = new StepDefinitionRepository(_context, _loggerFactory.CreateLogger<StepDefinitionRepository>()));
        }
    }
}
