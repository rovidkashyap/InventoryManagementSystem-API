using AutoMapper;
using IMS.Application.DTOs.Suppliers;
using IMS.Application.Services.SupplierServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.SupplierServices.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<SupplierService> _logger;
        private readonly IMemoryCache _cache;
        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SupplierService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> GetAllSuppliersCountAsync()
        {
            return await _unitOfWork.Suppliers.GetAllSuppliersCountAsync();
        }

        public async Task<IEnumerable<DisplaySuppliersDto>> GetAllSuppliersAsync()
        {
            const string cacheKey = "all_supplier";
            try
            {
                _logger.LogInformation("Fetching all suppliers.");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplaySuppliersDto> result))
                {
                    _logger.LogInformation("Returning cached suppliers.");
                    return result;
                }

                // If not in cache, fetch from database
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
                var supplierDto = _mapper.Map<IEnumerable<DisplaySuppliersDto>>(suppliers);

                // Cache the result
                _cache.Set(cacheKey, supplierDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return supplierDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all suppliers.");
                throw;
            }
        }

        public async Task<DisplaySuppliersDto> GetSupplierByIdAsync(Guid id)
        {
            var cacheKey = $"supplier_{id}";
            try
            {
                _logger.LogInformation($"Fetching supplier with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplaySuppliersDto cachedSupplier))
                {
                    _logger.LogInformation("Returning cached supplier");
                    return cachedSupplier;
                }

                // If not in cache, fetch from database
                var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
                if (supplier == null)
                {
                    _logger.LogWarning($"Supplier with ID: {id} was not found.");
                    return null;
                }

                var supplierDto = _mapper.Map<DisplaySuppliersDto>(supplier);

                // Cache the result
                _cache.Set(cacheKey, supplierDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return supplierDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the supplier with ID {id}.");
                throw;
            }
        }

        public async Task<CreateSupplierDto> CreateSupplierAsync(CreateSupplierDto supplierDto)
        {
            if (supplierDto == null)
            {
                throw new ArgumentNullException(nameof(supplierDto), "The supplier DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var supplier = _mapper.Map<Supplier>(supplierDto);

                // Log information about category creation
                _logger.LogInformation($"Creating a new supplier {supplierDto.SupplierName}");

                // Add the category to the repository
                await _unitOfWork.Suppliers.CreateAsync(supplier);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful supplier creation
                _logger.LogInformation($"Supplier with ID: {supplier.SupplierId} created successfully.");

                // Invalidate all cache for GetAllSupplierAsync
                _cache.Remove("all_supplier");

                // Map Entity back to DTO from return
                return _mapper.Map<CreateSupplierDto>(supplier);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the supplier.");
                throw new ApplicationException("An error occurred while adding the supplier", ex);
            }
        }

        public async Task<CreateSupplierDto> UpdateSupplierAsync(Guid id, CreateSupplierDto updateSupplierDto)
        {
            if (updateSupplierDto == null)
            {
                throw new ArgumentNullException(nameof(updateSupplierDto), "The updateSupplier Dto cannot be null.");
            }

            try
            {
                // Fetch the existing Supplier from the repository
                var existingSupplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

                if (existingSupplier == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing supplier with {id}");
                    throw new KeyNotFoundException($"supplier with ID {id} not found.");
                }

                // Log Information about the supplier before updating
                _logger.LogInformation($"Updating category with ID: {id} - Current Name: {existingSupplier.SupplierName}");

                // Map the updates from DTO to the existing supplier
                _mapper.Map(updateSupplierDto, existingSupplier);

                // Log the details of the update
                _logger.LogInformation($"Updatng category with ID: {id} - New Name: {updateSupplierDto.SupplierName}");

                // Update the supplier using unitofwork with supplierId
                await _unitOfWork.Suppliers.UpdateAsync(existingSupplier, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful supplier update
                _logger.LogInformation($"Supplier with ID: {id} update successfully.");

                // Invalidate caches for this supplier and all suppliers
                _cache.Remove($"supplier_{id}");
                _cache.Remove("all_supplier");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateSupplierDto>(existingSupplier);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the supplier with ID: {id}");
                throw new ApplicationException("An error occurred while updating the supplier", ex);
            }
        }

        public async Task<bool> DeleteSupplierAsync(Guid id)
        {
            try
            {
                // Fetch the existing supplier from the unitofwork
                var existingSupplier = await _unitOfWork.Suppliers.GetByIdAsync(id);

                if (existingSupplier == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing supplier with ID: {id}");
                    return false; // Return false if the supplier was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting supplier with ID: {id} - Name: {existingSupplier.SupplierName}");

                // Delete the supplier from the repository
                await _unitOfWork.Suppliers.DeleteAsync(id);

                // Invalidate caches for this supplier and all suppliers
                _cache.Remove($"supplier_{id}");
                _cache.Remove("all_supplier");

                // Log successful supplier deletion
                _logger.LogInformation($"Category with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the supplier with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the supplier", ex);
            }
        }

    }
}
