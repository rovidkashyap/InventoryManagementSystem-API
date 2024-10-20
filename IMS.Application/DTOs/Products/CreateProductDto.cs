namespace IMS.Application.DTOs.Products
{
    public class CreateProductDto
    {
        public Guid ProductId { get; set; }
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

    }
}
