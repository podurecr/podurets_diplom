using Repositories.Enums;

namespace Repositories.Entities
{
    public class ShipmentDecision
    {
        public int Id { get; set; }

        public int BatchId { get; set; }

        public Batch? Batch { get; set; }

        public ShipmentDecisionStatus Status { get; set; } = ShipmentDecisionStatus.NotCreated;

        public string DecisionText { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }
    }
}
