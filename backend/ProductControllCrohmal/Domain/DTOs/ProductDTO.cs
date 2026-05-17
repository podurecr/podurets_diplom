namespace Domain.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Unit { get; set; } = "кг";

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<BatchDTO> Batches { get; set; } = new List<BatchDTO>();

        public ICollection<ProductQualitySpecificationDTO> QualitySpecifications { get; set; } = new List<ProductQualitySpecificationDTO>();
    }
}
