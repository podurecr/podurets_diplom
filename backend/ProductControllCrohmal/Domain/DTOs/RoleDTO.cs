namespace Domain.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public ICollection<UserDTO> Users { get; set; } = new List<UserDTO>();

    }
}
