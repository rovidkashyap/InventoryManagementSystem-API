namespace IMS.Application.DTOs.Manufacturers
{
    public class DisplayManufacturerDto
    {
        public Guid ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Address { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
