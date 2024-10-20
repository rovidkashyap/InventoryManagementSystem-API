using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.TransactionTypeRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.TransactionTypeRepository.Repository
{
    public class TransactionTypeRepository : BaseRepository<TransactionType>, ITransactionTypeRepository
    {
        private readonly IMSContext _context;
        public TransactionTypeRepository(IMSContext context, ILogger<BaseRepository<TransactionType>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
