using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IShipmentDecisionRepository : IRepository<ShipmentDecision>
    {
        Task<ShipmentDecision?> GetByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task<ShipmentDecision?> GetByBatchIdNoTrackingAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ShipmentDecision>> GetAllWithDetailsAsync(
            CancellationToken cancellationToken = default);

        Task<ShipmentDecision?> GetByBatchIdForUpdateAsync(
            int batchId,
            CancellationToken cancellationToken = default);


    }
}
