namespace Domain.DTOs
{
    public class QualityParameterDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductQualitySpecificationDTO> ProductQualitySpecifications { get; set; } = new List<ProductQualitySpecificationDTO>();

        public ICollection<AnalysisResultDTO> AnalysisResults { get; set; } = new List<AnalysisResultDTO>();
    }
}
