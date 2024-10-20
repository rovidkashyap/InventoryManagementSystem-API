using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Category : AuditableEntity
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string CategoryName { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
