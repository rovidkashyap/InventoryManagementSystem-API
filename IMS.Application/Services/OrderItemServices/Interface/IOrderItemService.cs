using IMS.Application.DTOs.OrderItems;

namespace IMS.Application.Services.OrderItemServices.Interface
{
    public interface IOrderItemService
    {
        Task<IEnumerable<DisplayOrderItemDto>> GetAllOrderItemAsync();
        Task<DisplayOrderItemDto> GetOrderItemByIdAsync(Guid id);
        Task<DisplayOrderItemDto> CreateOrderItemAsync(CreateOrderItemDto orderItemDto);
        Task<DisplayOrderItemDto> UpdateOrderItemAsync(Guid id, UpdateOrderItemDto orderItemDto);
        Task<bool> DeleteOrderItemAsync(Guid id);
    }
}
