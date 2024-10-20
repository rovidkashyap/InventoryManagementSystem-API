using IMS.Core.Entities;
using IMS.DataAccess.Repositories.BaseRepository.Interface;

namespace IMS.DataAccess.Repositories.OrderRepository.Interface
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<int> GetAllOrdersCountAsync();
        Task<IEnumerable<Order>> GetAllOrdersWithOrderItemsAsync();
        Task<int> GetOrdersCountByOrderStatus(string statusName);
        Task<List<Order>> GetRecentOrdersAsync(int count);
    }
}
