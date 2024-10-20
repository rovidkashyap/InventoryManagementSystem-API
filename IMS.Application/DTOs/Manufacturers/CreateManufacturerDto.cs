namespace IMS.Application.DTOs.Manufacturers
{
    public class CreateManufacturerDto
    {
        public Guid ManufacturerId { get; set; }
        public string ManufacturerName { get; set; }
        public string? ContactInfo { get; set; }
        public string? Address { get; set; }
    }
}
