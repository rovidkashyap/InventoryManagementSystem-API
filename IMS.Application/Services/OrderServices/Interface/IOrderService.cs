using IMS.Application.DTOs.Orders;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace IMS.Application.Services.OrderServices.Interface
{
    public interface IOrderService
    {
        Task<IEnumerable<DisplayOrderDto>> GetAllOrderAsync();
        Task<DisplayOrderDto> GetOrderByIdAsync(Guid id);
        Task<DisplayOrderDto> CreateOrderAsync(CreateOrderDto orderDto);
        Task<DisplayOrderDto> UpdateOrderAsync(Guid id, UpdateOrderDto orderDto);
        Task<bool> DeleteOrderAsync(Guid id);
        Task<int> GetAllOrdersCountAsync();
        Task<IEnumerable<DisplayOrdersForOrderItemsDto>> GetAllOrdersWithOrderItemsAsync();
        Task<int> GetOrdersCountByOrderStatus(string statusName);

        Task<List<OrderDto>> GetRecentOrdersAsync(int count);
    }
}
