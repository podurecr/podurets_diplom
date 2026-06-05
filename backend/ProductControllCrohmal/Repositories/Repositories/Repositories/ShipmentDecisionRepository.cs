using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class ShipmentDecisionRepository : Repository<ShipmentDecision>, IShipmentDecisionRepository
    {
        private readonly ApplicationDbContext context;

        public ShipmentDecisionRepository(ApplicationDbContext context)
            : base(context)
        {
            this.context = context;
        }

        public async Task<ShipmentDecision?> GetByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await context.ShipmentDecisions
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .Include(x => x.CreatedByUser)
                    .ThenInclude(x => x!.Role)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }

        public async Task<ShipmentDecision?> GetByBatchIdNoTrackingAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await context.ShipmentDecisions
                .AsNoTracking()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .Include(x => x.CreatedByUser)
                    .ThenInclude(x => x!.Role)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }

        public async Task<IReadOnlyList<ShipmentDecision>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default)
        {
            return await context.ShipmentDecisions
                .AsNoTracking()
                .Include(x => x.Batch)
                    .ThenInclude(x => x!.Product)
                .Include(x => x.CreatedByUser)
                    .ThenInclude(x => x!.Role)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<ShipmentDecision?> GetByBatchIdForUpdateAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            return await context.ShipmentDecisions
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }
    }
}
