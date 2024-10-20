using IMS.Application.DTOs.OrderItems;

namespace IMS.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal NetAmount { get; set; }
        public Guid OrderStatusId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string BillingEmail { get; set; }
        public string BillingPhone { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? Notes { get; set; }
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }
}
