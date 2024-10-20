using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.ManufacturerRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.ManufacturerRepository.Repository
{
    public class ManufacturerRepository : BaseRepository<Manufacturer>, IManufacturerRepository
    {
        private readonly IMSContext _context;
        public ManufacturerRepository(IMSContext context, ILogger<BaseRepository<Manufacturer>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
