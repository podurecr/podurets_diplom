namespace Domain.DTOs
{
    public class QualityCertificateDTO
    {
        public int Id { get; set; }

        public string CertificateNumber { get; set; } = string.Empty;

        public int BatchId { get; set; }

        public BatchDTO? Batch { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserId { get; set; }

        public UserDTO? CreatedByUser { get; set; }

        public string Conclusion { get; set; } = string.Empty;

        public string? PdfPath { get; set; }
    }
}
