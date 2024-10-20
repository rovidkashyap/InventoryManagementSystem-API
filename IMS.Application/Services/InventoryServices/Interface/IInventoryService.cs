using IMS.Application.DTOs.Inventories;

namespace IMS.Application.Services.InventoryServices.Interface
{
    public interface IInventoryService
    {
        Task<IEnumerable<DisplayInventoryDto>> GetAllInventoriesAsync();
        Task<DisplayInventoryDto> GetInventoryByIdAsync(Guid id);
        Task<CreateInventoryDto> CreateInventoryAsync(CreateInventoryDto inventoryDto);
        Task<CreateInventoryDto> UpdateInventoryAsync(Guid id, CreateInventoryDto invenroryDto);
        Task<bool> DeleteInventoryAsync(Guid id);

        Task<int> GetAllInventoryCountAsync();
        Task<IEnumerable<DisplayRecentInventoryDto>> GetRecentInventories();
    }
}
