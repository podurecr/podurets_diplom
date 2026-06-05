namespace Repositories.Entities
{
    public class AnalysisResult
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public Batch? Batch { get; set; }

        public int QualityParameterId { get; set; }

        public QualityParameter? QualityParameter { get; set; }

        public decimal? NumericValue { get; set; }

        public string? TextValue { get; set; }

        public bool? IsWithinNorm { get; set; }

        public string? Comment { get; set; }
        public bool IsAnalysisCompleted { get; set; } = false;

        public DateTime? AnalysisCompletedAt { get; set; }

        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        public int EnteredByUserId { get; set; }

        public User? EnteredByUser { get; set; }
    }
}
