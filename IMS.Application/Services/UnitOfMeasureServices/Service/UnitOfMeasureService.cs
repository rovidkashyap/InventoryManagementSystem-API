using AutoMapper;
using IMS.Application.DTOs.Manufacturers;
using IMS.Application.DTOs.UnitOfMeasures;
using IMS.Application.Services.UnitOfMeasureServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.UnitOfMeasureServices.Service
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<UnitOfMeasureService> _logger;
        private readonly IMemoryCache _cache;
        public UnitOfMeasureService(IUnitOfWork unitOfWork, IMapper mapper, 
            ILogger<UnitOfMeasureService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayUnitOfMeasureDto>> GetAllUnitOfMeasureAsync()
        {
            const string cacheKey = "all_unitofmeasure";
            try
            {
                // Check if data is in cache memory
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayUnitOfMeasureDto> result))
                {
                    _logger.LogInformation("Returning cached Unit Of Measure.");
                    return result;
                }

                // If not in cache, fetch from database
                var unitofmeasure = await _unitOfWork.UnitOfMeasures.GetAllAsync();
                var unitofmeasureDto = _mapper.Map<IEnumerable<DisplayUnitOfMeasureDto>>(unitofmeasure);

                // Cache the result
                _cache.Set(cacheKey, unitofmeasureDto, TimeSpan.FromMinutes(5)); // Set Cache Duration

                return unitofmeasureDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the result.");
                throw;
            }
        }

        public async Task<DisplayUnitOfMeasureDto> GetUnitOfMeasureByIdAsync(Guid Id)
        {
            var cacheKey = $"unitofmeasure_{Id}";
            try
            {
                _logger.LogInformation($"Fetching unitofmeasure with ID: {Id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayUnitOfMeasureDto cachedUnitOfMeasure))
                {
                    _logger.LogInformation("Returning cached unitofmeasure");
                    return cachedUnitOfMeasure;
                }

                // If not in cache, fetch from database
                var unitofmeasure = await _unitOfWork.Manufacturers.GetByIdAsync(Id);
                if (unitofmeasure == null)
                {
                    _logger.LogWarning($"UnitofMeasure with ID: {Id} was not found.");
                    return null;
                }

                var unitOfMeasureDto = _mapper.Map<DisplayUnitOfMeasureDto>(unitofmeasure);

                // Cache the result
                _cache.Set(cacheKey, unitOfMeasureDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return unitOfMeasureDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the unitofmeasure with ID {Id}.");
                throw;
            }
        }

        public async Task<CreateUnitOfMeasureDto> CreateUnitOfMeasureAsync(CreateUnitOfMeasureDto unitOfMeasureDto)
        {
            if (unitOfMeasureDto == null)
            {
                throw new ArgumentNullException(nameof(unitOfMeasureDto), "The UnitOfMeasure DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var unitofmeasure = _mapper.Map<UnitOfMeasure>(unitOfMeasureDto);

                // Log information about unitofmeasure creation
                _logger.LogInformation($"Creating a new unitofmeasure {unitOfMeasureDto.UnitName}");

                // Add the unitofmeasure to the repository
                await _unitOfWork.UnitOfMeasures.CreateAsync(unitofmeasure);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful unitofmeasure creation
                _logger.LogInformation($"UnitOfMeasure with ID: {unitofmeasure.UnitOfMeasureId} created successfully.");

                // Invalidate all cache for GetAllUnitOfMeasureAsync
                _cache.Remove("all_unitofmeasure");

                // Map Entity back to DTO from return
                return _mapper.Map<CreateUnitOfMeasureDto>(unitofmeasure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the unitofmeasure.");
                throw new ApplicationException("An error occurred while adding the unitofmeasure", ex);
            }
        }

        public async Task<CreateUnitOfMeasureDto> UpdateUnitOfMeasureAsync(Guid Id, CreateUnitOfMeasureDto unitOfMeasureDto)
        {
            if (unitOfMeasureDto == null)
            {
                throw new ArgumentNullException(nameof(unitOfMeasureDto), "The update unitOfMeasure DTO cannot be null.");
            }

            try
            {
                // Fetch the existing unitofmeasure from the repository
                var existingunitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(Id);

                if (existingunitOfMeasure == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing unitofmeasure with {Id}");
                    throw new KeyNotFoundException($"UnitofMeasure with ID {Id} not found.");
                }

                // Log Information about the unitofmeasure before updating
                _logger.LogInformation($"Updating unitofmeasure with ID: {Id} - Current Name: {existingunitOfMeasure.UnitName}");

                // Map the updates from DTO to the existing unitofmeasure
                _mapper.Map(unitOfMeasureDto, existingunitOfMeasure);

                // Log the details of the update
                _logger.LogInformation($"Updatng unitofMeasure with ID: {Id} - New Name: {unitOfMeasureDto.UnitName}");

                // Update the unitofmeasure using unitofwork with categoryId
                await _unitOfWork.UnitOfMeasures.UpdateAsync(existingunitOfMeasure, Id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful unitofmeasure update
                _logger.LogInformation($"UnitofMeasure with ID: {Id} update successfully.");

                // Invalidate caches for this unitofmeasure and all unitofmeasures
                _cache.Remove($"unitofmeasure_{Id}");
                _cache.Remove("all_unitofmeasure");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateUnitOfMeasureDto>(existingunitOfMeasure);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the unitofmeasure with ID: {Id}");
                throw new ApplicationException("An error occurred while updating the unitofmeasure", ex);
            }
        }

        public async Task<bool> DeleteUnitOfMeasureAsync(Guid Id)
        {
            try
            {
                // Fetch the existing unitOfMeasure from the unitofwork
                var existingunitOfMeasure = await _unitOfWork.UnitOfMeasures.GetByIdAsync(Id);

                if (existingunitOfMeasure == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing unitOfMeasure with ID: {Id}");
                    return false; // Return false if the manufacturer was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting unitOfMeasure with ID: {Id} - Name: {existingunitOfMeasure.UnitName}");

                // Delete the unitOfMeasure from the repository
                await _unitOfWork.UnitOfMeasures.DeleteAsync(Id);

                // Invalidate caches for this unitOfMeasure and all unitOfMeasures
                _cache.Remove($"unitofmeasure_{Id}");
                _cache.Remove("all_unitofmeasure");

                // Log successful unitOfMeasure deletion
                _logger.LogInformation($"unitOfMeasure with ID: {Id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the unitOfMeasure with ID: {Id}");
                throw new ApplicationException("An error occurred while deleting the unitOfMeasure", ex);
            }
        }
    }
}
