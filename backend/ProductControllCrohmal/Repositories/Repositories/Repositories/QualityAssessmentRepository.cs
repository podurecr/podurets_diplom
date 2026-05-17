using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class QualityAssessmentRepository : Repository<QualityAssessment>, IQualityAssessmentRepository
    {
        public QualityAssessmentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<QualityAssessment?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                .Include(x => x.AssessedByUser)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }
    }
}
