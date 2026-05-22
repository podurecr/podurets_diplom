using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Services.Services
{
    public class QualityAssessmentService : IQualityAssessmentService
    {
        private readonly IQualityAssessmentRepository qualityAssessmentRepository;
        private readonly IBatchRepository batchRepository;
        private readonly IAnalysisResultRepository analysisResultRepository;
        private readonly IProductQualitySpecificationRepository productQualitySpecificationRepository;
        private readonly IUserRepository userRepository;

        public QualityAssessmentService(
            IQualityAssessmentRepository qualityAssessmentRepository,
            IBatchRepository batchRepository,
            IAnalysisResultRepository analysisResultRepository,
            IProductQualitySpecificationRepository productQualitySpecificationRepository,
            IUserRepository userRepository)
        {
            this.qualityAssessmentRepository = qualityAssessmentRepository;
            this.batchRepository = batchRepository;
            this.analysisResultRepository = analysisResultRepository;
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
            this.userRepository = userRepository;
        }

        public async Task<QualityAssessmentDTO?> GetAssessmentByBatchIdAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            var assessment = await qualityAssessmentRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (assessment is null)
                return null;

            return EntityToDTOMapper.ToQualityAssessmentDTO(assessment);
        }

        public async Task<QualityAssessmentDTO> AssessBatchAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            if (userId <= 0)
                throw new ArgumentException("Некорректный ID пользователя.");

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            if (batch is null)
                throw new InvalidOperationException("Партия не найдена.");

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                throw new InvalidOperationException("Пользователь не найден.");

            var calculatedStatus = await CalculateBatchStatusAsync(batchId, cancellationToken);

            var isApproved = calculatedStatus == BatchStatus.Approved;

            var conclusion = !string.IsNullOrWhiteSpace(dto.Conclusion)
                ? dto.Conclusion.Trim()
                : isApproved
                    ? "Партія відповідає встановленим показникам якості."
                    : "Партія не відповідає встановленим показникам якості.";

            var existingAssessment = await qualityAssessmentRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            QualityAssessment assessment;

            if (existingAssessment is null)
            {
                assessment = new QualityAssessment
                {
                    BatchId = batchId,
                    IsApproved = isApproved,
                    Conclusion = conclusion,
                    AssessedAt = DateTime.UtcNow,
                    AssessedByUserId = userId
                };

                await qualityAssessmentRepository.AddAsync(assessment, cancellationToken);
            }
            else
            {
                assessment = existingAssessment;

                assessment.IsApproved = isApproved;
                assessment.Conclusion = conclusion;
                assessment.AssessedAt = DateTime.UtcNow;
                assessment.AssessedByUserId = userId;

                qualityAssessmentRepository.Update(assessment);
            }

            batch.Status = calculatedStatus;
            batchRepository.Update(batch);

            await qualityAssessmentRepository.SaveChangesAsync(cancellationToken);

            assessment.Batch = batch;
            assessment.AssessedByUser = user;

            return EntityToDTOMapper.ToQualityAssessmentDTO(assessment);
        }

        public async Task<BatchStatus> CalculateBatchStatusAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {
            if (batchId <= 0)
                throw new ArgumentException("Некорректный ID партии.");

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            if (batch is null)
                throw new InvalidOperationException("Партия не найдена.");

            var specifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(batch.ProductId, cancellationToken);

            var requiredSpecifications = specifications
                .Where(x => x.IsRequired)
                .ToList();

            if (requiredSpecifications.Count == 0)
                throw new InvalidOperationException("Для продукта не настроены обязательные спецификации качества.");

            var analysisResults = await analysisResultRepository
                .GetByBatchIdAsync(batchId, cancellationToken);

            if (analysisResults.Count == 0)
                return BatchStatus.Rejected;

            foreach (var specification in requiredSpecifications)
            {
                var result = analysisResults.FirstOrDefault(x =>
                    x.QualityParameterId == specification.QualityParameterId);

                if (result is null)
                    return BatchStatus.Rejected;

                if (result.IsWithinNorm != true)
                    return BatchStatus.Rejected;
            }

            return BatchStatus.Approved;
        }
    }
}
