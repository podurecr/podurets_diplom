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


    }
}
