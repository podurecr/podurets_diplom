using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories.Repositories.Interfaces
{
    public interface IQualityCertificateRepository : IRepository<QualityCertificate>
    {
        Task<QualityCertificate?> GetByBatchIdAsync(int batchId, CancellationToken cancellationToken = default);
        Task<QualityCertificate?> GetByCertificateNumberAsync(string certificateNumber, CancellationToken cancellationToken = default);
    }
}
