using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.UnitOfMeasureRepository.Interface;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.UnitOfMeasureRepository.Repository
{
    public class UnitOfMeasureRepository : BaseRepository<UnitOfMeasure>, IUnitOfMeasureRepository
    {
        private readonly IMSContext _context;
        public UnitOfMeasureRepository(IMSContext context, ILogger<BaseRepository<UnitOfMeasure>> logger) 
            : base(context, logger)
        {
            _context = context;
        }
    }
}
