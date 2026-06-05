using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs
{
    public class SaveBatchAnalysisRequestDTO
    {
        public int BatchId { get; set; }

        public List<SaveAnalysisResultItemDTO> Results { get; set; } = new();
    }

    public class SaveAnalysisResultItemDTO
    {
        public int QualityParameterId { get; set; }

        public decimal? NumericValue { get; set; }

        public string? TextValue { get; set; }

        public bool? IsWithinNorm { get; set; }

        public string? ResultStatus { get; set; }

        public string? Comment { get; set; }
    }
}
