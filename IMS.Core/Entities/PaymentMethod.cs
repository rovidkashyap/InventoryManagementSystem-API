using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class PaymentMethod : AuditableEntity
    {
        public Guid PaymentMethodId { get; set; } = Guid.NewGuid();
        public string PaymentMethodName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
