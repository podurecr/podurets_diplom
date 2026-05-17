using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IAnalysisResultRepository : IRepository<AnalysisResult>
    {
        Task<IReadOnlyList<AnalysisResult>> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);
    }
}
