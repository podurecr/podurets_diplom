using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class BatchRepository : Repository<Batch>, IBatchRepository
    {
        public BatchRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Batch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Product)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.BatchNumber == batchNumber, cancellationToken);
        }

        public async Task<Batch?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Product)
                .Include(x => x.CreatedByUser)
                .Include(x => x.AnalysisResults)
                    .ThenInclude(x => x.QualityParameter)
                .Include(x => x.QualityAssessment)
                .Include(x => x.QualityCertificate)
                .Include(x => x.ShipmentDecision)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Batch>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Product)
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Batch>> GetBatchesAllowedForShipmentAsync(
           CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Product)
                .Include(x => x.QualityCertificate)
                .Include(x => x.ShipmentDecision)
                .Where(x =>
                    x.IsAnalysisCompleted &&
                    x.QualityCertificate != null &&
                    (
                        x.Status == BatchStatus.Approved ||
                        x.Status == BatchStatus.ReadyForShipment
                    ))
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
