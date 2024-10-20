using AutoMapper;
using IMS.Application.DTOs.Customers;
using IMS.Application.DTOs.Inventories;
using IMS.Application.Services.CustomerServices.Service;
using IMS.Application.Services.InventoryServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.InventoryServices.Service
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<InventoryService> _logger;
        private IMemoryCache _cache;
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InventoryService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> GetAllInventoryCountAsync()
        {
            return await _unitOfWork.Inventories.GetAllInventoryCountAsync();
        }

        public async Task<IEnumerable<DisplayInventoryDto>> GetAllInventoriesAsync()
        {
            const string cacheKey = "all_inventory";
            try
            {
                _logger.LogInformation("Fetching all inventories");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayInventoryDto> result))
                {
                    _logger.LogInformation("Returning cached inventory.");
                    return result;
                }

                // If Not in cache, fetch from database
                var inventories = await _unitOfWork.Inventories.GetAllInventoriesIncludeDetails();
                var inventoryDto = _mapper.Map<IEnumerable<DisplayInventoryDto>>(inventories);

                // Cache the result
                _cache.Set(cacheKey, inventoryDto, TimeSpan.FromMinutes(5));  // Set appropriate cache duration

                return inventoryDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all inventories");
                throw;
            }
        }

        public async Task<IEnumerable<DisplayRecentInventoryDto>> GetRecentInventories()
        {
            try
            {
                _logger.LogInformation("Fetching Recent Inventories");

                var inventories = await _unitOfWork.Inventories.GetRecentInventories();
                var inventoriesDto = _mapper.Map<IEnumerable<DisplayRecentInventoryDto>>(inventories);

                return inventoriesDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching recent inventories");
                throw;
            }
        }

        public async Task<DisplayInventoryDto> GetInventoryByIdAsync(Guid id)
        {
            var cacheKey = $"inventory_{id}";
            try
            {
                _logger.LogInformation($"Fetching inventory with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayInventoryDto cachedInventory))
                {
                    _logger.LogInformation("Returning cached inventories");
                    return cachedInventory;
                }

                // If not in cache, fetch from database
                var inventory = await _unitOfWork.Inventories.GetByIdAsync(id);
                if (inventory == null)
                {
                    _logger.LogWarning($"Inventory with ID: {id} was not found.");
                    return null;
                }

                var inventoryDto = _mapper.Map<DisplayInventoryDto>(inventory);

                // Cache the result
                _cache.Set(cacheKey, inventoryDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return inventoryDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the inventory with ID {id}.");
                throw;
            }
        }

        public async Task<CreateInventoryDto> CreateInventoryAsync(CreateInventoryDto inventoryDto)
        {
            if (inventoryDto == null)
            {
                throw new ArgumentNullException(nameof(inventoryDto), "The inventory DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var inventory = _mapper.Map<Inventory>(inventoryDto);

                // Log Information about inventory creation
                _logger.LogInformation($"Creating a new inventory: {inventoryDto.InventoryId}");

                // Add the inventory to the repository
                await _unitOfWork.Inventories.CreateAsync(inventory);

                // Save changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successfull inventory creation
                _logger.LogInformation($"Inventory with ID: {inventory.InventoryId} created successfully.");

                // Invalidate all cache for GetAllInventoryAsync
                _cache.Remove("all_inventory");

                // Map Entity back to DTO for return
                return _mapper.Map<CreateInventoryDto>(inventory);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the inventory.");
                throw new ApplicationException("An error occured while adding the inventory", ex);
            }
        }

        public async Task<CreateInventoryDto> UpdateInventoryAsync(Guid id, CreateInventoryDto inventoryDto)
        {
            if (inventoryDto == null)
            {
                throw new ArgumentNullException(nameof(inventoryDto), "The inventory Dto cannot be null.");
            }

            try
            {
                // Fetch the existing inventory from the repository
                var existingInventory = await _unitOfWork.Inventories.GetByIdAsync(id);

                if (existingInventory == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing inventory with {id}");
                    throw new KeyNotFoundException($"Inventory with ID {id} not found.");
                }

                // Log Information about the inventory before updating
                _logger.LogInformation($"Updating inventory with ID: {id}");

                // Map the updates from DTO to the existing inventory
                _mapper.Map(inventoryDto, existingInventory);

                // Log the details of the update
                _logger.LogInformation($"Updatng inventory with ID: {id}");

                // Update the inventory using unitofwork with inventoryId
                await _unitOfWork.Inventories.UpdateAsync(existingInventory, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful inventory update
                _logger.LogInformation($"Inventory with ID: {id} update successfully.");

                // Invalidate caches for this inventory and all inventories
                _cache.Remove($"inventory_{id}");
                _cache.Remove("all_inventory");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateInventoryDto>(existingInventory);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the inventory with ID: {id}");
                throw new ApplicationException("An error occurred while updating the inventory", ex);
            }
        }

        public async Task<bool> DeleteInventoryAsync(Guid id)
        {
            try
            {
                // Fetch the existing inventory from the unitofwork
                var existingInventory = await _unitOfWork.Inventories.GetByIdAsync(id);

                if (existingInventory == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing inventory with ID: {id}");
                    return false; // Return false if the inventory was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting inventory with ID: {id}");

                // Delete the inventory from the repository
                await _unitOfWork.Inventories.DeleteAsync(id);

                // Invalidate caches for this inventory and all inventories
                _cache.Remove($"inventory_{id}");
                _cache.Remove("all_inventory");

                // Log successful inventory deletion
                _logger.LogInformation($"Inventory with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the inventory with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the inventory", ex);
            }
        }
    }
}
