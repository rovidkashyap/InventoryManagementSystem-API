using AutoMapper;
using IMS.Application.DTOs.Products;
using IMS.Application.Services.ProductServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.ProductServices.Service
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<ProductService> _logger;
        private IMemoryCache _cache;
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> GetAllProductsCountAsync()
        {
            try
            {
                return await _unitOfWork.Products.GetAllProductsCountAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred during processing request.");
                throw new ArgumentException("Failed to get products count.", ex);
            }
        }

        public async Task<IEnumerable<DisplayProductDto>> GetAllProductsAsync()
        {
            const string cacheKey = "all_products";
            try
            {
                _logger.LogInformation("Fetching all products");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayProductDto> result))
                {
                    _logger.LogInformation("Returning cached products.");
                    return result;
                }

                // If Not in cache, fetch from database
                var products = await _unitOfWork.Products.GetAllProductsIncludeDetails();
                var productDto = _mapper.Map<IEnumerable<DisplayProductDto>>(products);

                // Cache the result
                _cache.Set(cacheKey, productDto, TimeSpan.FromMinutes(5));  // Set appropriate cache duration

                return productDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all products");
                throw;
            }
        }

        public async Task<DisplayProductDto> GetProductByIdAsync(Guid id)
        {
            var cacheKey = $"product_{id}";
            try
            {
                _logger.LogInformation($"Fetching product with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayProductDto cachedProduct))
                {
                    _logger.LogInformation("Returning cached product");
                    return cachedProduct;
                }

                // If not in cache, fetch from database
                var product = await _unitOfWork.Products.GetProductByIdIncludeDetails(id);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID: {id} was not found.");
                    return null;
                }

                var productDto = _mapper.Map<DisplayProductDto>(product);

                // Cache the result
                _cache.Set(cacheKey, productDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the product with ID {id}.");
                throw;
            }
        }

        public async Task<CreateProductDto> CreateProductAsync(CreateProductDto productDto)
        {
            if(productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "The product DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var product = _mapper.Map<Product>(productDto);

                // Log Information about product creation
                _logger.LogInformation($"Creating a new product: {productDto.ProductName}");

                // Add the product to the repository
                await _unitOfWork.Products.CreateAsync(product);

                // Save changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successfull product creation
                _logger.LogInformation($"Product with ID: {product.ProductId} created successfully.");

                // Invalidate all cache for GetAllProductsAsync
                _cache.Remove("all_products");

                // Map Entity back to DTO for return
                return _mapper.Map<CreateProductDto>(product);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the product.");
                throw new ApplicationException("An error occured while adding the product", ex);
            }
        }

        public async Task<CreateProductDto> UpdateProductAsync(Guid id, CreateProductDto productDto)
        {
            if (productDto == null)
            {
                throw new ArgumentNullException(nameof(productDto), "The product Dto cannot be null.");
            }

            try
            {
                // Fetch the existing Product from the repository
                var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing product with {id}");
                    throw new KeyNotFoundException($"Product with ID {id} not found.");
                }

                // Log Information about the product before updating
                _logger.LogInformation($"Updating product with ID: {id} - Current Name: {existingProduct.ProductName}");

                // Map the updates from DTO to the existing product
                _mapper.Map(productDto, existingProduct);

                // Log the details of the update
                _logger.LogInformation($"Updatng product with ID: {id} - New Name: {productDto.ProductName}");

                // Update the product using unitofwork with productId
                await _unitOfWork.Products.UpdateAsync(existingProduct, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful product update
                _logger.LogInformation($"Product with ID: {id} update successfully.");

                // Invalidate caches for this product and all products
                _cache.Remove($"product_{id}");
                _cache.Remove("all_product");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateProductDto>(existingProduct);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the product with ID: {id}");
                throw new ApplicationException("An error occurred while updating the product", ex);
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            try
            {
                // Fetch the existing product from the unitofwork
                var existingProduct = await _unitOfWork.Products.GetByIdAsync(id);

                if (existingProduct == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing product with ID: {id}");
                    return false; // Return false if the product was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting product with ID: {id} - Name: {existingProduct.ProductName}");

                // Delete the product from the repository
                await _unitOfWork.Products.DeleteAsync(id);

                // Invalidate caches for this product and all products
                _cache.Remove($"product_{id}");
                _cache.Remove("all_product");

                // Log successful product deletion
                _logger.LogInformation($"Product with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the product with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the product", ex);
            }
        }

        public async Task<IEnumerable<ProductsByManufacturerDto>> GetProductsByManufacturerIdAsync(Guid manufacturerId)
        {
            var cacheKey = $"product_{manufacturerId}";
            try
            {
                _logger.LogInformation($"Fetching Product with ID: {manufacturerId}");
                
                //Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out IEnumerable<ProductsByManufacturerDto> result))
                {
                    _logger.LogInformation("Returned cached product");
                    return result;
                }

                // If not in cache, fetch from database
                var products = await _unitOfWork.Products.GetProductsByManufacturerId(manufacturerId);
                if (products == null)
                {
                    _logger.LogWarning($"Product with ID: {manufacturerId} was not found.");
                    return null;
                }

                var productDto = _mapper.Map<IEnumerable<ProductsByManufacturerDto>>(products);

                // Cache the result
                _cache.Set(cacheKey, productDto, TimeSpan.FromMinutes(5));

                return productDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the product with ID {manufacturerId}.");
                throw;
            }   
        }
    }
}
