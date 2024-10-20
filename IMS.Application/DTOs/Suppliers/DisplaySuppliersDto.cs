namespace IMS.Application.DTOs.Suppliers
{
    public class DisplaySuppliersDto
    {
        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string ContactName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
    }
}
