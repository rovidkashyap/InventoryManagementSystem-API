using IMS.Application.DTOs.OrderStatus;

namespace IMS.Application.Services.OrderStatusServices.Interface
{
    public interface IOrderStatusService
    {
        Task<IEnumerable<DisplayOrderStatusDto>> GetAllOrderStatusAsync();
        Task<DisplayOrderStatusDto> GetOrderStatusByIdAsync(Guid id);
        Task<DisplayOrderStatusDto> CreateOrderStatusAsync(CreateOrderStatusDto orderStatusDto);
        Task<DisplayOrderStatusDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto orderStatusDto);
        Task<bool> DeleteOrderStatusAsync(Guid id);
    }
}
