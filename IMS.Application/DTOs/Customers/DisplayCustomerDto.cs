using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace IMS.Application.DTOs.Customers
{
    public class DisplayCustomerDto
    {
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; } 
        public string City { get; set; } 
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
