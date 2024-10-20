using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Supplier : AuditableEntity
    {
        public Guid SupplierId { get; set; } = Guid.NewGuid();
        public string SupplierName { get; set; }  // Name of the supplier
        public string ContactName { get; set; }  // Name of the primary contact person at the supplier
        public string PhoneNumber { get; set; }  // Phone number for the supplier
        public string Email { get; set; }  // Email address for the supplier
        public string Address { get; set; }  // Physical address of the supplier
        public string City { get; set; }  // City where the supplier is located
        public string State { get; set; }  // State where the supplier is located
        public string PostalCode { get; set; }  // Postal code of the supplier's address
        public string Country { get; set; }  // Country where the supplier is located
        public string? Notes { get; set; }  // Optional notes or additional information about the supplier

        // Navigation Property
        public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();  // Collection of related Inventory records
    }
}
