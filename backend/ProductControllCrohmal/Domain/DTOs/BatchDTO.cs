using Repositories.Enums;

namespace Domain.DTOs
{
    public class BatchDTO
    {
        public int Id { get; set; }

        public string BatchNumber { get; set; } = string.Empty;

        public DateTime ProductionDate { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; } = "кг";

        public string? ProductionLine { get; set; }

        public string? Comment { get; set; }

        public BatchStatus Status { get; set; } = BatchStatus.Registered;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int ProductId { get; set; }

        public ProductDTO? Product { get; set; }

        public int CreatedByUserId { get; set; }

        public UserDTO? CreatedByUser { get; set; }

        public ICollection<AnalysisResultDTO> AnalysisResults { get; set; } = new List<AnalysisResultDTO>();

        public QualityAssessmentDTO? QualityAssessment { get; set; }

        public QualityCertificateDTO? QualityCertificate { get; set; }

        public ShipmentDecisionDTO? ShipmentDecision { get; set; }
    }
}
