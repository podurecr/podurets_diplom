namespace Domain.DTOs
{
    public class QualityAssessmentDTO
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public BatchDTO? Batch { get; set; }

        public bool IsApproved { get; set; }

        public string Conclusion { get; set; } = string.Empty;

        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;

        public int AssessedByUserId { get; set; }

        public UserDTO? AssessedByUser { get; set; }
    }
}
