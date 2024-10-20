using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.CategoryRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.CategoryRepository.Repository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly IMSContext _context;
        public CategoryRepository(IMSContext context, ILogger<BaseRepository<Category>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
