using Microsoft.EntityFrameworkCore;
using Repositories.Data;
using Repositories.Entities;
using Repositories.Repositories.Interfaces;

namespace Repositories.Repositories.Repositories
{
    public class ShipmentDecisionRepository : Repository<ShipmentDecision>, IShipmentDecisionRepository
    {
        public ShipmentDecisionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ShipmentDecision?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default)
        {
            return await QueryNoTracking()
                .Include(x => x.Batch)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.BatchId == batchId, cancellationToken);
        }
    }
}
