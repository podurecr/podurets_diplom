using Domain.DTOs;
using Repositories.Entities;
using Repositories.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Mappers
{
    public static class DTOToEntityMapper
    {
        public static User ToUserEntity(UserDTO dto, string passwordHash)
        {
            return new User
            {
                FullName = dto.FullName.Trim(),
                Login = dto.Login.Trim(),
                Email = dto.Email.Trim(),
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.SpecifyKind(dto.CreatedAt, DateTimeKind.Utc),
                RoleId = dto.RoleId
            };
        }

        public static Role ToRoleEntity(RoleDTO dto)
        {
            return new Role
            {
                Name = dto.Name.Trim()
            };
        }

        public static Product ToProductEntity(ProductDTO dto)
        {
            return new Product
            {
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim(),
                Unit = dto.Unit.Trim(),
                Description = dto.Description,
                IsActive = true
            };
        }

        public static Batch ToBatchEntity(BatchDTO dto, int userId)
        {
            return new Batch
            {
                BatchNumber = dto.BatchNumber.Trim(),
                ProductionDate = dto.ProductionDate,
                Quantity = dto.Quantity,
                Unit = dto.Unit.Trim(),
                ProductionLine = dto.ProductionLine,
                Comment = dto.Comment,
                Status = BatchStatus.Registered,
                CreatedAt = DateTime.SpecifyKind(dto.CreatedAt, DateTimeKind.Utc),
                ProductId = dto.ProductId,
                CreatedByUserId = userId,
                IsAnalysisCompleted = dto.IsAnalysisCompleted
            };
        }

        public static QualityParameter ToQualityParameterEntity(QualityParameterDTO dto)
        {
            return new QualityParameter
            {
                Name = dto.Name.Trim(),
                Unit = dto.Unit.Trim(),
                Description = dto.Description,
                IsActive = true
            };
        }

        public static ProductQualitySpecification ToProductQualitySpecificationEntity(ProductQualitySpecificationDTO dto)
        {
            return new ProductQualitySpecification
            {
                ProductId = dto.ProductId,
                QualityParameterId = dto.QualityParameterId,
                MinValue = dto.MinValue,
                MaxValue = dto.MaxValue,
                TextNorm = dto.TextNorm,
                IsRequired = dto.IsRequired
            };
        }

        public static AnalysisResult ToAnalysisResultEntity(
            int batchId,
            AnalysisResultDTO dto,
            int userId)
        {
            return new AnalysisResult
            {
                BatchId = batchId,
                QualityParameterId = dto.QualityParameterId,
                NumericValue = dto.NumericValue,
                TextValue = dto.TextValue,
                IsWithinNorm = dto.IsWithinNorm,
                Comment = dto.Comment,
                AnalyzedAt = DateTime.SpecifyKind(dto.AnalyzedAt, DateTimeKind.Utc),
                EnteredByUserId = userId,
                IsAnalysisCompleted = dto.IsAnalysisCompleted,
                AnalysisCompletedAt = dto.AnalysisCompletedAt,
            };
        }

        public static QualityAssessment ToQualityAssessmentEntity(
            int batchId,
            int userId,
            bool isApproved,
            string conclusion,
            bool isFinal,
            DateTime AssessedAt)
        {
            return new QualityAssessment
            {
                BatchId = batchId,
                IsApproved = isApproved,
                Conclusion = conclusion,
                AssessedAt = DateTime.SpecifyKind(AssessedAt, DateTimeKind.Utc),
                AssessedByUserId = userId,
                IsFinal = isFinal
            };
        }

        public static QualityCertificate ToQualityCertificateEntity(
            int batchId,
            int userId,
            string certificateNumber,
            string conclusion,
            DateTime cratedAt,
            string? pdfPath = null
        )
        {
            return new QualityCertificate
            {
                CertificateNumber = certificateNumber,
                BatchId = batchId,
                CreatedAt = DateTime.SpecifyKind(cratedAt, DateTimeKind.Utc),
                CreatedByUserId = userId,
                Conclusion = conclusion,
                PdfPath = pdfPath
            };
        }

        public static ShipmentDecision ToShipmentDecisionEntity(
            int batchId,
            int userId,
            ShipmentDecisionStatus status,
            string decisionText,
            DateTime cratedAt)
        {
            return new ShipmentDecision
            {
                BatchId = batchId,
                Status = status,
                DecisionText = decisionText,
                CreatedAt = DateTime.SpecifyKind(cratedAt, DateTimeKind.Utc),
                CreatedByUserId = userId
            };
        }
    }
}
