using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.OrderStatusRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.OrderStatusRepository.Repository
{

    public class OrderStatusRepository : BaseRepository<OrderStatus>, IOrderStatusRepository
    {
        private readonly IMSContext _context;
        public OrderStatusRepository(IMSContext context, ILogger<BaseRepository<OrderStatus>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
