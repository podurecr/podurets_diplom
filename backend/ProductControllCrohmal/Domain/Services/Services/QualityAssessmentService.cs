using Domain.DTOs;
using Domain.Mappers;
using Domain.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Repositories.Entities;
using Repositories.Enums;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using static QuestPDF.Helpers.Colors;

namespace Domain.Services.Services
{
    public class QualityAssessmentService : IQualityAssessmentService
    {
        private readonly IQualityAssessmentRepository qualityAssessmentRepository;
        private readonly IBatchRepository batchRepository;
        private readonly IAnalysisResultRepository analysisResultRepository;
        private readonly IProductQualitySpecificationRepository productQualitySpecificationRepository;
        private readonly IUserRepository userRepository;
        private readonly IQualityCertificateService qualityCertificateService;
        private readonly IProductRepository productRepository;

        public QualityAssessmentService(
            IQualityAssessmentRepository qualityAssessmentRepository,
            IBatchRepository batchRepository,
            IAnalysisResultRepository analysisResultRepository,
            IProductQualitySpecificationRepository productQualitySpecificationRepository,
            IUserRepository userRepository,
            IQualityCertificateService qualityCertificateService, IProductRepository productRepository)
        {
            this.qualityAssessmentRepository = qualityAssessmentRepository;
            this.batchRepository = batchRepository;
            this.analysisResultRepository = analysisResultRepository;
            this.productQualitySpecificationRepository = productQualitySpecificationRepository;
            this.userRepository = userRepository;
            this.qualityCertificateService = qualityCertificateService;
            this.productRepository = productRepository;
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

            return loadProductAndBatch(EntityToDTOMapper.ToQualityAssessmentDTO(assessment));
        }

        public async Task<QualityAssessmentDTO> AssessBatchAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default)
        {

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);


            var user = await userRepository.GetByIdAsync(userId, cancellationToken);


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

            return loadProductAndBatch(EntityToDTOMapper.ToQualityAssessmentDTO(assessment));
        }

        public async Task<BatchStatus> CalculateBatchStatusAsync(
            int batchId,
            CancellationToken cancellationToken = default)
        {

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);

            var specifications = await productQualitySpecificationRepository
                .GetByProductIdAsync(batch.ProductId, cancellationToken);

            var requiredSpecifications = specifications
                .Where(x => x.IsRequired)
                .ToList();

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

        public async Task<QualityAssessmentDTO> SaveAssessmentAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return  loadProductAndBatch(await SaveOrFinalizeAssessmentAsync(
                batchId,
                dto,
                userId,
                isFinal: false,
                cancellationToken));
        }

        public async Task<QualityAssessmentDTO> FinalizeAssessmentAsync(
            int batchId,
            QualityAssessmentDTO dto,
            int userId,
            CancellationToken cancellationToken = default)
        {
            return loadProductAndBatch(await SaveOrFinalizeAssessmentAsync(
                batchId,
                dto,
                userId,
                isFinal: true,
                cancellationToken));
        }

        private async Task<QualityAssessmentDTO> SaveOrFinalizeAssessmentAsync(
          int batchId,
          QualityAssessmentDTO dto,
          int userId,
          bool isFinal,
          CancellationToken cancellationToken)
        {

            var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);


            var user = await userRepository.GetByIdAsync(userId, cancellationToken);


            var assessment = await qualityAssessmentRepository
                .GetByBatchIdForUpdateAsync(batchId, cancellationToken);


            if (assessment is null)
            {
                assessment = new QualityAssessment
                {
                    BatchId = batchId,
                    IsApproved = dto.IsApproved,
                    Conclusion = dto.Conclusion.Trim(),
                    AssessedAt = DateTime.UtcNow,
                    AssessedByUserId = userId,
                    IsFinal = false
                };

                await qualityAssessmentRepository.AddAsync(assessment, cancellationToken);
            }
            else
            {
                assessment.IsApproved = dto.IsApproved;
                assessment.Conclusion = dto.Conclusion.Trim();
                assessment.AssessedAt = DateTime.UtcNow;
                assessment.AssessedByUserId = userId;
            }

            if (isFinal)
            {
                assessment.IsFinal = true;

                batch.Status = dto.IsApproved
                    ? BatchStatus.Approved
                    : BatchStatus.Rejected;
            }
           

            await qualityAssessmentRepository.SaveChangesAsync(cancellationToken);

            if (isFinal && dto.IsApproved)
            {
                await qualityCertificateService.GenerateCertificateAsync(
                    batchId,
                    userId,
                    cancellationToken);
            }

            var analysisResults = await analysisResultRepository
                .Query()
                .Where(x => x.BatchId == batchId)
                .ToListAsync(cancellationToken);

            foreach (var decision in dto.ResultDecisions)
            {
                var analysisResult = analysisResults.FirstOrDefault(x =>
                    x.QualityParameterId == decision.QualityParameterId);

                if (analysisResult is null)
                    continue;

                analysisResult.IsWithinNorm = decision.IsWithinNorm;

                analysisResultRepository.Update(analysisResult);
            }

            await analysisResultRepository.SaveChangesAsync(cancellationToken);

            return EntityToDTOMapper.ToQualityAssessmentDTO(assessment);
        }

        private QualityAssessmentDTO loadProductAndBatch(QualityAssessmentDTO qualityAssessmentDTO)
        {
            qualityAssessmentDTO.AssessedByUser = EntityToDTOMapper.ToUserDTO(userRepository.GetByIdAsync(qualityAssessmentDTO.AssessedByUserId).Result);
            var batch = EntityToDTOMapper.ToBatchDTO(batchRepository.GetByIdAsync(qualityAssessmentDTO.BatchId).Result);
            batch.Product = EntityToDTOMapper.ToProductDTO(productRepository.GetByIdAsync(batch.ProductId).Result);

            qualityAssessmentDTO.Batch = batch;
            return qualityAssessmentDTO;
        }
    }
}
