using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Inventory : AuditableEntity
    {
        public Guid InventoryId { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Guid SupplierId { get; set; }
        public int QuantityAdded { get; set; }
        public int QuantityRemoved { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid TransactionTypeId { get; set; }
        public string? Remark { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual Product Product { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual TransactionType TransactionType { get; set; }
    }
}
