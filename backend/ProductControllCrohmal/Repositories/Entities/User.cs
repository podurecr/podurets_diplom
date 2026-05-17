namespace Repositories.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int RoleId { get; set; }

        public Role? Role { get; set; }

        public ICollection<Batch> CreatedBatches { get; set; } = new List<Batch>();

        public ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();

        public ICollection<QualityAssessment> QualityAssessments { get; set; } = new List<QualityAssessment>();

        public ICollection<QualityCertificate> QualityCertificates { get; set; } = new List<QualityCertificate>();

        public ICollection<ShipmentDecision> ShipmentDecisions { get; set; } = new List<ShipmentDecision>();
    }
}
