namespace IMS.Application.DTOs.Products
{
    public class ProductsByManufacturerDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public decimal? Weight { get; set; }
        public string Dimensions { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsActive { get; set; }
    }
}
