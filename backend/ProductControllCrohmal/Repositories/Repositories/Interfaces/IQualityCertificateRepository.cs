using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IQualityCertificateRepository : IRepository<QualityCertificate>
    {
        Task<IReadOnlyList<QualityCertificate>> GetAllWithDetailsAsync(
                   CancellationToken cancellationToken = default);

       Task<QualityCertificate?> GetByIdWithDetailsAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<QualityCertificate?> GetByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);

       Task<QualityCertificate?> GetByBatchIdNoTrackingAsync(
            int batchId,
            CancellationToken cancellationToken = default);
    }
}
