namespace IMS.Application.DTOs.OrderItems
{
    public class DisplayOrderItemsForOrdersDto
    {
        public Guid OrderItemId { get; set; }   // Primary Key
        public string ProductName { get; set; } // Foreign Key to Product Entity
        public int Quantity { get; set; }   // Quantity of the product Ordered
        public decimal UnitPrice { get; set; }  // Price per unit at the time of the order
        public decimal TotalPrice { get; set; } // Calculated as Quantity * UnitPrice
        public decimal DiscountAmount { get; set; } // Discount on item
        public decimal TaxAmount { get; set; } // Tax for Item
        public decimal NetPrice { get; set; } // Total after Discount, Tax Applied
    }
}
