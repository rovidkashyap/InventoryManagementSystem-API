using AutoMapper;
using IMS.Application.DTOs.Manufacturers;
using IMS.Application.Services.ManufacturerServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.ManufacturerServices.Service
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<ManufacturerService> _logger;
        private readonly IMemoryCache _cache;
        public ManufacturerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ManufacturerService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayManufacturerDto>> GetAllManufacturersAsync()
        { 
            const string cacheKey = "all_manufacturer";
            try
            {
                // Check if data is in cache memory
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayManufacturerDto> result))
                {
                    _logger.LogInformation("Returning cached Manufacturer.");
                    return result;
                }

                // If not in cache, fetch from database
                var manufacturer = await _unitOfWork.Manufacturers.GetAllAsync();
                var manufacturerDto = _mapper.Map<IEnumerable<DisplayManufacturerDto>>(manufacturer);

                // Cache the result
                _cache.Set(cacheKey, manufacturerDto, TimeSpan.FromMinutes(5)); // Set Cache Duration

                return manufacturerDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the result.");
                throw;
            }
        }

        public async Task<DisplayManufacturerDto> GetManufacturerByIdAsync(Guid id)
        {
            var cacheKey = $"manufacturer_{id}";
            try
            {
                _logger.LogInformation($"Fetching manufacturer with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayManufacturerDto cachedManufacturer))
                {
                    _logger.LogInformation("Returning cached manufaturer");
                    return cachedManufacturer;
                }

                // If not in cache, fetch from database
                var manufacturer = await _unitOfWork.Manufacturers.GetByIdAsync(id);
                if (manufacturer == null)
                {
                    _logger.LogWarning($"Manufacturer with ID: {id} was not found.");
                    return null;
                }

                var manufacturerDto = _mapper.Map<DisplayManufacturerDto>(manufacturer);

                // Cache the result
                _cache.Set(cacheKey, manufacturerDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return manufacturerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the manufacturer with ID {id}.");
                throw;
            }
        }

        public async Task<CreateManufacturerDto> CreateManufacturerAsync(CreateManufacturerDto manufacturerDto)
        {
            if (manufacturerDto == null)
            {
                throw new ArgumentNullException(nameof(manufacturerDto), "The Manufacturer DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var manufacturer = _mapper.Map<Manufacturer>(manufacturerDto);

                // Log information about manufacturer creation
                _logger.LogInformation($"Creating a new manufacturer {manufacturerDto.ManufacturerName}");

                // Add the manufacturer to the repository
                await _unitOfWork.Manufacturers.CreateAsync(manufacturer);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful category creation
                _logger.LogInformation($"Manufacturer with ID: {manufacturer.ManufacturerId} created successfully.");

                // Invalidate all cache for GetAllManufacturerAsync
                _cache.Remove("all_manufacturer");

                // Map Entity back to DTO from return
                return _mapper.Map<CreateManufacturerDto>(manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                throw new ApplicationException("An error occurred while adding the manufacturer", ex);
            }
        }

        public async Task<CreateManufacturerDto> UpdateManufacturerAsync(Guid Id, CreateManufacturerDto manufacturerDto)
        {
            if (manufacturerDto == null)
            {
                throw new ArgumentNullException(nameof(manufacturerDto), "The update manufacturer DTO cannot be null.");
            }

            try
            {
                // Fetch the existing category from the repository
                var existingManufacturer = await _unitOfWork.Manufacturers.GetByIdAsync(Id);

                if (existingManufacturer == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing manufacturer with {Id}");
                    throw new KeyNotFoundException($"Manufacturer with ID {Id} not found.");
                }

                // Log Information about the manufacturer before updating
                _logger.LogInformation($"Updating manufacturer with ID: {Id} - Current Name: {existingManufacturer.ManufacturerName}");

                // Map the updates from DTO to the existing category
                _mapper.Map(manufacturerDto, existingManufacturer);

                // Log the details of the update
                _logger.LogInformation($"Updatng manufacturer with ID: {Id} - New Name: {manufacturerDto.ManufacturerName}");

                // Update the manufacturer using unitofwork with categoryId
                await _unitOfWork.Manufacturers.UpdateAsync(existingManufacturer, Id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful manufacturer update
                _logger.LogInformation($"Manufacturer with ID: {Id} update successfully.");

                // Invalidate caches for this manufacturer and all manufacturers
                _cache.Remove($"manufacturer_{Id}");
                _cache.Remove("all_manufacturer");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateManufacturerDto>(existingManufacturer);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the manufacturer with ID: {Id}");
                throw new ApplicationException("An error occurred while updating the manufacturer", ex);
            }
        }

        public async Task<bool> DeleteManufacturerAsync(Guid Id)
        {
            try
            {
                // Fetch the existing manufacturer from the unitofwork
                var existingManufacturer = await _unitOfWork.Manufacturers.GetByIdAsync(Id);

                if (existingManufacturer == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing manufacturer with ID: {Id}");
                    return false; // Return false if the manufacturer was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting manufacturer with ID: {Id} - Name: {existingManufacturer.ManufacturerName}");

                // Delete the manufacturer from the repository
                await _unitOfWork.Manufacturers.DeleteAsync(Id);

                // Invalidate caches for this manufacturer and all manufacturers
                _cache.Remove($"manufacturer_{Id}");
                _cache.Remove("all_manufacturer");

                // Log successful manufacturer deletion
                _logger.LogInformation($"Manufacturer with ID: {Id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the manufacturer with ID: {Id}");
                throw new ApplicationException("An error occurred while deleting the manufacturer", ex);
            }
        }
    }
}
