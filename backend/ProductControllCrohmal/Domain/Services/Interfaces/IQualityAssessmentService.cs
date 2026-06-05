using Domain.DTOs;
using Repositories.Entities;
using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Interfaces
{
    public interface IQualityAssessmentService
    {
        Task<QualityAssessmentDTO> AssessBatchAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default);

        Task<QualityAssessmentDTO?> GetAssessmentByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task<BatchStatus> CalculateBatchStatusAsync(
            int batchId,
            CancellationToken cancellationToken = default);

        Task<QualityAssessmentDTO> SaveAssessmentAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default);

        Task<QualityAssessmentDTO> FinalizeAssessmentAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default);
    }
}
