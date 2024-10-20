using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Product : AuditableEntity
    {
        public Guid ProductId { get; set; } = Guid.NewGuid();
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Guid ManufacturerId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public decimal? Weight { get; set; }
        public string Dimensions { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
        public string? ImagePath { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }
        public virtual Category Category { get; set; }
        public virtual UnitOfMeasure UnitOfMeasure { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
