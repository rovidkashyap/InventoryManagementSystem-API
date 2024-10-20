using AutoMapper;
using IMS.Application.DTOs.Categories;
using IMS.Application.Services.CategoryServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.CategoryServices.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<CategoryService> _logger;
        private readonly IMemoryCache _cache;
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CategoryService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayCategoryDto>> GetAllCategoryAsync()
        {
            const string cacheKey = "all_categories";
            try
            {
                _logger.LogInformation("Fetching all categories.");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayCategoryDto> result))
                {
                    _logger.LogInformation("Returning cached categories.");
                    return result;
                }

                // If not in cache, fetch from database
                var categories = await _unitOfWork.Categories.GetAllAsync();
                var categoryDto = _mapper.Map<IEnumerable<DisplayCategoryDto>>(categories);

                // Cache the result
                _cache.Set(cacheKey, categoryDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return categoryDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all categories.");
                throw;
            }
        }

        public async Task<DisplayCategoryDto> GetCategoryByIdAsync(Guid id)
        {
            var cacheKey = $"category_{id}";
            try
            {
                _logger.LogInformation($"Fetching category with ID: {id}");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out DisplayCategoryDto cachedCategory))
                {
                    _logger.LogInformation("Returning cached category");
                    return cachedCategory;
                }

                // If not in cache, fetch from database
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                if(category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} was not found.");
                    return null;
                }
                
                var categoryDto = _mapper.Map<DisplayCategoryDto>(category);

                // Cache the result
                _cache.Set(cacheKey, categoryDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return categoryDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the category with ID {id}.");
                throw;
            }
        }

        public async Task<CreateCategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            if(categoryDto == null)
            {
                throw new ArgumentNullException(nameof(categoryDto), "The category DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var category = _mapper.Map<Category>(categoryDto);

                // Log information about category creation
                _logger.LogInformation($"Creating a new category {categoryDto.CategoryName}");

                // Add the category to the repository
                await _unitOfWork.Categories.CreateAsync(category);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful category creation
                _logger.LogInformation($"Category with ID: {category.CategoryId} created successfully.");

                // Invalidate all cache for GetAllCategoryAsync
                _cache.Remove("all_categories");

                // Map Entity back to DTO from return
                return _mapper.Map<CreateCategoryDto>(category);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the category.");
                throw new ApplicationException("An error occurred while adding the category", ex);
            }
        }

        public async Task<CreateCategoryDto> UpdateCategoryAsync(Guid categoryId, CreateCategoryDto updateCategoryDto)
        {
            if(updateCategoryDto == null)
            {
                throw new ArgumentNullException(nameof(updateCategoryDto), "The update category DTO cannot be null.");
            }

            try
            {
                // Fetch the existing category from the repository
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);

                if(existingCategory == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing category with {categoryId}");
                    throw new KeyNotFoundException($"Category with ID {categoryId} not found.");
                }

                // Log Information about the category before updating
                _logger.LogInformation($"Updating category with ID: {categoryId} - Current Name: {existingCategory.CategoryName}");

                // Map the updates from DTO to the existing category
                _mapper.Map(updateCategoryDto, existingCategory);

                // Log the details of the update
                _logger.LogInformation($"Updatng category with ID: {categoryId} - New Name: {updateCategoryDto.CategoryName}");

                // Update the category using unitofwork with categoryId
                await _unitOfWork.Categories.UpdateAsync(existingCategory, categoryId);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful category update
                _logger.LogInformation($"Category with ID: {categoryId} update successfully.");

                // Invalidate caches for this category and all categories
                _cache.Remove($"category_{categoryId}");
                _cache.Remove("all_categories");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateCategoryDto>(existingCategory);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the category with ID: {categoryId}");
                throw new ApplicationException("An error occurred while updating the category", ex);
            }
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            try
            {
                // Fetch the existing category from the unitofwork
                var existingCategory = await _unitOfWork.Categories.GetByIdAsync(categoryId);

                if(existingCategory == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing category with ID: {categoryId}");
                    return false; // Return false if the category was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting category with ID: {categoryId} - Name: {existingCategory.CategoryName}");

                // Delete the category from the repository
                await _unitOfWork.Categories.DeleteAsync(categoryId);

                // Invalidate caches for this category and all categories
                _cache.Remove($"category_{categoryId}");
                _cache.Remove("all_categories");

                // Log successful category deletion
                _logger.LogInformation($"Category with ID: {categoryId} deleted successfully.");

                return true;
            }
            catch(Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the category with ID: {categoryId}");
                throw new ApplicationException("An error occurred while deleting the category", ex);
            }
        }
    }
}
