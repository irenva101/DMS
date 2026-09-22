using CSharpFunctionalExtensions;
using DMS.Library.Data;
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
    }
}
