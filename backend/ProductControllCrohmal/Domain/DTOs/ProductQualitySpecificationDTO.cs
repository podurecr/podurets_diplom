namespace Domain.DTOs
{
    public class ProductQualitySpecificationDTO
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public ProductDTO? Product { get; set; }

        public int QualityParameterId { get; set; }

        public QualityParameterDTO? QualityParameter { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public string? TextNorm { get; set; }

        public bool IsRequired { get; set; } = true;
    }
}
