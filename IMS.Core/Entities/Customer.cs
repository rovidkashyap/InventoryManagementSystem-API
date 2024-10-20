using IMS.Core.BaseEntity;

namespace IMS.Core.Entities
{
    public class Customer : AuditableEntity
    {
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }  // Full name of the customer
        public string Email { get; set; }  // Email address of the customer
        public string? PhoneNumber { get; set; }  // Contact phone number of the customer
        public string? WhatsAppNumber { get; set; }  // WhatsApp number of the customer
        public string Gender { get; set; }  // Gender of the customer (e.g., Male, Female, Other)
        public string Address { get; set; }  // Physical address of the customer
        public string City { get; set; }  // City where the customer resides
        public string State { get; set; }  // State where the customer resides
        public string PostalCode { get; set; }  // Postal code for the customer's address
        public string Country { get; set; }  // Country where the customer resides
        public DateTime DateOfBirth { get; set; }  // Date of birth of the customer
        public string? Notes { get; set; }  // Optional notes or additional information about the customer

        // Navigation Property
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();  // Collection of related Orders
    }
}
