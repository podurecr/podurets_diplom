using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class QualityCertificateRepository : Repository<QualityCertificate>, IQualityCertificateRepository
    {
        public QualityCertificateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<QualityCertificate?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }

        public async Task<QualityCertificate?> GetByCertificateNumberAsync(
            string certificateNumber,
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.CertificateNumber == certificateNumber, cancellationToken);
        }
    }
}
