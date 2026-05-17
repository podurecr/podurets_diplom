namespace Repositories.Entities
{
    public class ProductQualitySpecification
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int QualityParameterId { get; set; }

        public QualityParameter? QualityParameter { get; set; }

        public decimal? MinValue { get; set; }

        public decimal? MaxValue { get; set; }

        public string? TextNorm { get; set; }

        public bool IsRequired { get; set; } = true;
    }
}
