namespace Domain.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }

        public RoleDTO? Role { get; set; }

        public ICollection<BatchDTO> CreatedBatches { get; set; } = new List<BatchDTO>();

        public ICollection<AnalysisResultDTO> AnalysisResults { get; set; } = new List<AnalysisResultDTO>();

        public ICollection<QualityAssessmentDTO> QualityAssessments { get; set; } = new List<QualityAssessmentDTO>();

        public ICollection<QualityCertificateDTO> QualityCertificates { get; set; } = new List<QualityCertificateDTO>();

        public ICollection<ShipmentDecisionDTO> ShipmentDecisions { get; set; } = new List<ShipmentDecisionDTO>();
    }
}
