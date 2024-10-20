using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.OrderItemRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.OrderItemRepository.Repository
{
    public class OrderItemRepository : BaseRepository<OrderItem>, IOrderItemRepository
    {
        private readonly IMSContext _context;
        public OrderItemRepository(IMSContext context, ILogger<BaseRepository<OrderItem>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
