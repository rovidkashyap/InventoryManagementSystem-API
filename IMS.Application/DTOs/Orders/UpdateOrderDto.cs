using IMS.Application.DTOs.OrderItems;

namespace IMS.Application.DTOs.Orders
{
    public class UpdateOrderDto
    {
        public Guid OrderId { get; set; }   // Primary Key for the order
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public Guid OrderStatusId { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string ShippingAddress { get; set; }
        public string? BillingAddress { get; set; }
        public string BillingEmail { get; set; }
        public string BillingPhone { get; set; }
        public DateTime ShippingDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string? Notes { get; set; }
        public List<UpdateOrderItemDto> OrderItems { get; set; } = new List<UpdateOrderItemDto>();
    }
}
