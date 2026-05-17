using Domain.DTOs;
using Repositories.Entities;

namespace Domain.Mappers
{
    public static class EntityToDTOMapper
    {
        public static UserDTO ToUserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Login = user.Login,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                RoleId = user.RoleId,
            };
        }

        public static ProductDTO ToProductDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Unit = product.Unit,
                Description = product.Description,
                IsActive = product.IsActive
            };
        }

        public static BatchDTO ToBatchDTO(Batch batch)
        {
            return new BatchDTO
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                ProductionDate = batch.ProductionDate,
                Quantity = batch.Quantity,
                Unit = batch.Unit,
                ProductionLine = batch.ProductionLine,
                Status = batch.Status,
                CreatedAt = batch.CreatedAt,
                ProductId = batch.ProductId,
                CreatedByUserId = batch.CreatedByUserId,
            };
        }

        public static ProductQualitySpecificationDTO ToSpecificationDTO(ProductQualitySpecification specification)
        {
            return new ProductQualitySpecificationDTO
            {
                Id = specification.Id,
                ProductId = specification.ProductId,
                QualityParameterId = specification.QualityParameterId,
                MinValue = specification.MinValue,
                MaxValue = specification.MaxValue,
                TextNorm = specification.TextNorm,
                IsRequired = specification.IsRequired
            };
        }

        public static AnalysisResultDTO ToAnalysisResultDTO(AnalysisResult result)
        {
            return new AnalysisResultDTO
            {
                Id = result.Id,
                BatchId = result.BatchId,
                QualityParameterId = result.QualityParameterId,
                NumericValue = result.NumericValue,
                TextValue = result.TextValue,
                IsWithinNorm = result.IsWithinNorm,
                Comment = result.Comment,
                AnalyzedAt = result.AnalyzedAt,
                EnteredByUserId = result.EnteredByUserId,
            };
        }

        public static QualityAssessmentDTO ToQualityAssessmentDTO(QualityAssessment assessment)
        {
            return new QualityAssessmentDTO
            {
                Id = assessment.Id,
                BatchId = assessment.BatchId,
                IsApproved = assessment.IsApproved,
                Conclusion = assessment.Conclusion,
                AssessedAt = assessment.AssessedAt,
                AssessedByUserId = assessment.AssessedByUserId,
            };
        }

        public static QualityCertificateDTO ToQualityCertificateDTO(QualityCertificate certificate)
        {
            return new QualityCertificateDTO
            {
                Id = certificate.Id,
                CertificateNumber = certificate.CertificateNumber,
                BatchId = certificate.BatchId,
                CreatedAt = certificate.CreatedAt,
                CreatedByUserId = certificate.CreatedByUserId,
                Conclusion = certificate.Conclusion,
                PdfPath = certificate.PdfPath
            };
        }

        public static ShipmentDecisionDTO ToShipmentDecisionDTO(ShipmentDecision decision)
        {
            return new ShipmentDecisionDTO
            {
                Id = decision.Id,
                BatchId = decision.BatchId,
                Status = decision.Status,
                DecisionText = decision.DecisionText,
                CreatedAt = decision.CreatedAt,
                CreatedByUserId = decision.CreatedByUserId,
            };
        }
    }
}