using Repositories.Enums;

namespace Repositories.Entities
{
    public class Batch
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

        public Product? Product { get; set; }

        public int CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }

        public ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();

        public QualityAssessment? QualityAssessment { get; set; }

        public QualityCertificate? QualityCertificate { get; set; }

        public ShipmentDecision? ShipmentDecision { get; set; }
    }
}
