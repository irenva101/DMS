using CSharpFunctionalExtensions;
using DMS.Library.Data;
using DMS.Library.Repositories.Interfaces;
using DMS.Shared.Commons;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DMS.Library.Repositories.Implementations
{
    public class CodeSequenceRepository : ICodeSequenceRepository
    {
        protected readonly DmsContext _dbContext;
        protected readonly ILogger<CodeSequenceRepository> _logger;

        public CodeSequenceRepository(DmsContext dbContext, ILogger<CodeSequenceRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<int, ResponseError>> GetNextValue(string prefix, int year)
        {
            try
            {
                // Upsert so the first code of a new prefix or year needs no seeded row;
                // the row lock serializes concurrent callers until their transaction ends
                var values = await _dbContext.Database
                    .SqlQuery<int>($"""
                        INSERT INTO dms.code_sequences (prefix, year, last_value)
                        VALUES ({prefix}, {year}, 1)
                        ON CONFLICT (prefix, year) DO UPDATE SET last_value = code_sequences.last_value + 1
                        RETURNING last_value AS "Value"
                        """)
                    .ToListAsync();

                return values.Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{ErrorMessage}", ex.Message);
                return new ResponseError(ResponseErrorCode.Exception, "An error occurred.");
            }
        }
    }
}
