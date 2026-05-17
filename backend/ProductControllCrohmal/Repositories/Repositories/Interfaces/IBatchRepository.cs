using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IBatchRepository : IRepository<Batch>
    {
        Task<Batch?> GetByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default);
        Task<Batch?> GetWithDetailsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Batch>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    }
}
