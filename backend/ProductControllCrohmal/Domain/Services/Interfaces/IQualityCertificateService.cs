using Domain.DTOs;
using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IQualityCertificateService
    {
        Task<QualityCertificateDTO> GenerateCertificateAsync(
            int batchId,
            int userId,
            CancellationToken cancellationToken = default);

        Task<QualityCertificateDTO?> GetCertificateByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task<byte[]> ExportCertificateToPdfAsync(
            int certificateId,
            CancellationToken cancellationToken = default);
    }
}
