using Domain.DTOs;
using Repositories.Entities;
using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IBatchService
    {
        Task<List<BatchDTO>> GetBatchesAsync(
            BatchDTO filter,
            CancellationToken cancellationToken = default);

        Task<BatchDTO?> GetBatchByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<BatchDTO> CreateBatchAsync(
            BatchDTO dto,
            int userId,
            CancellationToken cancellationToken = default);

        Task<BatchDTO> UpdateBatchAsync(
            int id,
            BatchDTO dto,
            CancellationToken cancellationToken = default);

        Task ChangeStatusAsync(
            int batchId,
            BatchStatus status,
            CancellationToken cancellationToken = default);
    }
}
