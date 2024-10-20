using IMS.Application.DTOs.Products;

namespace IMS.Application.Services.ProductServices.Interface
{
    public interface IProductService
    {
        Task<IEnumerable<DisplayProductDto>> GetAllProductsAsync();
        Task<DisplayProductDto> GetProductByIdAsync(Guid id);
        Task<CreateProductDto> CreateProductAsync(CreateProductDto productDto);
        Task<CreateProductDto> UpdateProductAsync(Guid id, CreateProductDto productDto);
        Task<bool> DeleteProductAsync(Guid id);

        Task<IEnumerable<ProductsByManufacturerDto>> GetProductsByManufacturerIdAsync(Guid manufacturerId);
        Task<int> GetAllProductsCountAsync();
    }
}
