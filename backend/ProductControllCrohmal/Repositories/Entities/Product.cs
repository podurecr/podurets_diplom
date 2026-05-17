namespace Repositories.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = "кг";

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Batch> Batches { get; set; } = new List<Batch>();

        public ICollection<ProductQualitySpecification> QualitySpecifications { get; set; } = new List<ProductQualitySpecification>();
    }
}
