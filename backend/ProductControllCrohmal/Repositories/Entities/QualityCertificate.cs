namespace Repositories.Entities
{
    public class QualityCertificate
    {
        public int Id { get; set; }

        public string CertificateNumber { get; set; } = string.Empty;

        public int BatchId { get; set; }

        public Batch? Batch { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }

        public string Conclusion { get; set; } = string.Empty;

        public string? PdfPath { get; set; }
    }
}
