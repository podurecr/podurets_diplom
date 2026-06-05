using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Repositories
{
    public class AnalysisResultRepository : Repository<AnalysisResult>, IAnalysisResultRepository
    {
        public AnalysisResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<AnalysisResult>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.QualityParameter)
                .Include(x => x.EnteredByUser)
                .Where(x => x.BatchId == batchId)
                .OrderBy(x => x.QualityParameter!.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<AnalysisResult>> GetByBatchIdForUpdateAsync(
           int batchId,
           CancellationToken cancellationToken = default)
        {
            return await Query()
                .Where(x => x.BatchId == batchId)
                .ToListAsync(cancellationToken);
        }
    }
}
