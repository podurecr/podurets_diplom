namespace Repositories.Entities
{
    public class QualityAssessment
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public Batch? Batch { get; set; }

        public bool IsApproved { get; set; }

        public string Conclusion { get; set; } = string.Empty;

        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

        public int AssessedByUserId { get; set; }

        public User? AssessedByUser { get; set; }
    }
}
