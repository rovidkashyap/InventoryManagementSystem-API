using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Manufacturer : AuditableEntity
    {
        public Guid ManufacturerId { get; set; } = Guid.NewGuid();
        public string ManufacturerName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Address { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
