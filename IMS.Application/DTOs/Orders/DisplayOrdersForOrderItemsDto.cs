using IMS.Application.DTOs.OrderItems;

namespace IMS.Application.DTOs.Orders
{
    public class DisplayOrdersForOrderItemsDto
    {
        public Guid OrderId { get; set; }   // Primary Key
        public string CustomerName { get; set; }    // Foreign Key to Customer entity
        public DateTime OrderDate { get; set; } // Date when the order was placed
        public decimal TotalPrice { get; set; } // Total Amount for the order
        public decimal TaxAmount { get; set; }  // Tax amount
        public decimal DiscountAmount { get; set; } // Discounted Amount
        public decimal ShippingCost { get; set; } // Shipping Charges
        public decimal NetAmount { get; set; } // Final Amount after Tax, Discount, Shipping
        public string OrderStatusName { get; set; } // Status of the order (e.g., Pending, Shipped, Completed) 
        public string PaymentMethodName { get; set; }   // Method of Payment (e.g, Credit Card, Paytm)
        public string ShippingAddress { get; set; } // Address where the order should be shipped
        public string? BillingAddress { get; set; } // Address where bill has been send
        public string BillingEmail { get; set; } // Email Id where contact for billing
        public string BillingPhone { get; set; } // Mobile Number where contact for billing
        public DateTime ShippingDate { get; set; } // Date on which order has been shipped
        public DateTime? DeliveredDate { get; set; } // Date on which order is delivered
        public string? Notes { get; set; }  // Optional Notes or instruction for the order

        public List<DisplayOrderItemsForOrdersDto> OrderItems { get; set; } = new List<DisplayOrderItemsForOrdersDto>();
    }
}
