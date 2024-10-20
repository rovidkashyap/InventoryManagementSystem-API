using IMS.Core.Entities;
using IMS.DataAccess.Repositories.BaseRepository.Interface;

namespace IMS.DataAccess.Repositories.InventoryRepository.Interface
{
    public interface IInventoryRepository : IBaseRepository<Inventory>
    {
        Task<IEnumerable<Inventory>> GetAllInventoriesIncludeDetails();
        Task<int> GetAllInventoryCountAsync();
        Task<IEnumerable<Inventory>> GetRecentInventories();
    }
}
