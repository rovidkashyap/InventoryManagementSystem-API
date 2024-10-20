using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class OrderStatus : AuditableEntity
    {
        public Guid OrderStatusId { get; set; } = Guid.NewGuid();
        public string OrderStatusName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}