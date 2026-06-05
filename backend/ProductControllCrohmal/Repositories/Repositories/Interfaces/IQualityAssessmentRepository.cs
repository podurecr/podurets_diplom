using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IQualityAssessmentRepository : IRepository<QualityAssessment>
    {
        Task<QualityAssessment?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);
        Task<QualityAssessment?> GetByBatchIdNoTrackingAsync(int batchId, CancellationToken cancellationToken = default);
        Task<QualityAssessment?> GetByBatchIdForUpdateAsync(
            int batchId,
            CancellationToken cancellationToken = default);
    }
}
