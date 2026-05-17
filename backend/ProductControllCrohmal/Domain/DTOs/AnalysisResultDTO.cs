namespace Domain.DTOs
{
    public class AnalysisResultDTO
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public BatchDTO? Batch { get; set; }

        public int QualityParameterId { get; set; }

        public QualityParameterDTO? QualityParameter { get; set; }

        public decimal? NumericValue { get; set; }

        public string? TextValue { get; set; }

        public bool? IsWithinNorm { get; set; }

        public string? Comment { get; set; }

        public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;

        public int EnteredByUserId { get; set; }

        public UserDTO? EnteredByUser { get; set; }
    }
}
