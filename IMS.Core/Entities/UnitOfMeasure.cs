using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class UnitOfMeasure : AuditableEntity
    {
        public Guid UnitOfMeasureId { get; set; } = Guid.NewGuid();
        public string UnitName { get; set; }
        public string? Abbreviation { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
