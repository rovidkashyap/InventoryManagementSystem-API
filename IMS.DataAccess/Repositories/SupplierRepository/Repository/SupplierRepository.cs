using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.SupplierRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace IMS.DataAccess.Repositories.SupplierRepository.Repository
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        private readonly IMSContext _context;
        public SupplierRepository(IMSContext context, ILogger<BaseRepository<Supplier>> logger) 
            : base(context, logger)
        {
            _context = context;
        }

        public async Task<int> GetAllSuppliersCountAsync()
        {
            return await _context.Suppliers.CountAsync();
        }
    }
}
