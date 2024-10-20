using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class TransactionType : AuditableEntity
    {
        public Guid TransactionTypeId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }    // (e.g., Purchase, Sale, Return, Adjustment)
        public string? Description { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}