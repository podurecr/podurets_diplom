using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IShipmentDecisionService
    {
        Task<ShipmentDecisionDTO> CreateDecisionAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<List<BatchDTO>> GetBatchesAllowedForShipmentAsync(
            CancellationToken cancellationToken = default);

        Task<ShipmentDecisionDTO?> GetDecisionByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);
    }
}
