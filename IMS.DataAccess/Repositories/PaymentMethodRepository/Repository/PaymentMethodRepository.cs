using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.PaymentMethodRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.PaymentMethodRepository.Repository
{
    public class PaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
    {
        private readonly IMSContext _context;
        public PaymentMethodRepository(IMSContext context, ILogger<BaseRepository<PaymentMethod>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
