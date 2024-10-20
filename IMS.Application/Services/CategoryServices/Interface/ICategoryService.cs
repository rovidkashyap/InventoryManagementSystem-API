using IMS.Application.DTOs.Categories;

namespace IMS.Application.Services.CategoryServices.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<DisplayCategoryDto>> GetAllCategoryAsync();
        Task<DisplayCategoryDto> GetCategoryByIdAsync(Guid id);
        Task<CreateCategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto);
        Task<CreateCategoryDto> UpdateCategoryAsync(Guid categoryId, CreateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
    }
}
