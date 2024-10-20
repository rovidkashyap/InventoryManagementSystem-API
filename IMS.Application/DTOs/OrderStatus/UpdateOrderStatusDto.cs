using System.Security.Principal;

namespace IMS.Application.DTOs.OrderStatus
{
    public class UpdateOrderStatusDto
    {
        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public string? Description { get; set; }
    }
}
