using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.InventoryRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.InventoryRepository.Repository
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly IMSContext _context;
        public InventoryRepository(IMSContext context, ILogger<BaseRepository<Inventory>> logger) 
            : base(context, logger)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoriesIncludeDetails()
        {
            return await _context.Inventories
                .Include(x => x.Product)
                .Include(x => x.Supplier)
                .Include(x => x.TransactionType)
                .ToListAsync();
        }

        public async Task<int> GetAllInventoryCountAsync()
        {
            return await _context.Inventories.CountAsync();
        }

        public async Task<IEnumerable<Inventory>> GetRecentInventories()
        {
            return await _context.Inventories
                .Include(x => x.TransactionType)
                .Include(x => x.Supplier)
                .Include(x => x.Product)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();
        }
    }
}
