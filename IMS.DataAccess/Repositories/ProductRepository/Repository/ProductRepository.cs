using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.ProductRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.ProductRepository.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly IMSContext _context;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(IMSContext context, ILogger<ProductRepository> logger) 
            : base(context, logger)
        {
            _context = context;
        }

        public async Task<int> GetAllProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsIncludeDetails()
        {
            return await _context.Products
                .Include(x => x.Manufacturer)
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .ToListAsync();
        }

        public async Task<Product> GetProductByIdIncludeDetails(Guid id)
        {
            return await _context.Products
                .Include(x => x.Manufacturer)
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .SingleOrDefaultAsync(x => x.ProductId == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByManufacturerId(Guid manufacturerId)
        {
            return await _context.Products
                .Include(x => x.Category)
                .Include(x => x.UnitOfMeasure)
                .Where(x => x.ManufacturerId == manufacturerId)
                .ToListAsync();
        }
    }
}
