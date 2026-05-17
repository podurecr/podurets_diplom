using Repositories.Enums;

namespace Domain.DTOs
{
    public class ShipmentDecisionDTO
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public BatchDTO? Batch { get; set; }

        public ShipmentDecisionStatus Status { get; set; } = ShipmentDecisionStatus.NotCreated;

        public string DecisionText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserId { get; set; }

        public UserDTO? CreatedByUser { get; set; }
    }
}
