using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.OrderRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.OrderRepository.Repository
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly IMSContext _context;
        public OrderRepository(IMSContext context, ILogger<BaseRepository<Order>> logger) 
            : base(context, logger)
        {
            _context = context;
        }

        public async Task<int> GetAllOrdersCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetOrdersCountByOrderStatus(string statusName)
        {
            var count = await _context.Orders.Where(x => x.OrderStatus.OrderStatusName == statusName).CountAsync();
            return count;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersWithOrderItemsAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.Customer)
                .Include(o => o.PaymentMethod)
                .Include(o => o.OrderStatus)
                .ToListAsync();
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count)
        {
            return await _context.Orders
                .Include(x => x.Customer)
                .Include(x => x.OrderStatus)
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .ToListAsync();
        }
    }
}
