using IMS.Core.Entities;
using IMS.DataAccess.Repositories.BaseRepository.Interface;

namespace IMS.DataAccess.Repositories.SupplierRepository.Interface
{
    public interface ISupplierRepository : IBaseRepository<Supplier>
    {
        Task<int> GetAllSuppliersCountAsync();
    }
}
