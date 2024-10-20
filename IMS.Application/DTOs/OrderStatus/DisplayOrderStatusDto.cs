using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace IMS.Application.DTOs.OrderStatus
{
    public class DisplayOrderStatusDto
    {
        public Guid OrderStatusId { get; set; }
        public string OrderStatusName { get; set; }
        public string Description { get; set; }
    }
}
