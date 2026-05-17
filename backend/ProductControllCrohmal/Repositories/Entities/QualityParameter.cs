namespace Repositories.Entities
{
    public class QualityParameter
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ProductQualitySpecification> ProductQualitySpecifications { get; set; } = new List<ProductQualitySpecification>();

        public ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
    }
}
