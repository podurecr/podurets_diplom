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

        public async Task<IReadOnlyList<QualityCertificate>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<QualityCertificate?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<QualityCertificate?> GetByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await Query()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }

        public async Task<QualityCertificate?> GetByBatchIdNoTrackingAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }

        public async Task<bool> ExistsForBatchAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .AnyAsync(x => x.BatchId == batchId, cancellationToken);
        }
    }
}
