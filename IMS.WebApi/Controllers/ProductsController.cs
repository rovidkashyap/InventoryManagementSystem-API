using IMS.Application.DTOs.Products;
using IMS.Application.Services.ProductServices.Interface;
using IMS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private ILogger<ProductsController> _logger;
        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: api/products/GetAllProductsCount
        [HttpGet("GetAllProductsCount")]
        public async Task<IActionResult> GetAllProductsCount()
        {
            try
            {
                var count = await _productService.GetAllProductsCountAsync();
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "Products Count retrieved successfully."
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products count.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        //GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Received request to get all products.");
            try
            {
                var products = await _productService.GetAllProductsAsync();

                if (products == null || !products.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all products.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            _logger.LogInformation($"Recieved request to get Product with Id: {id}");

            try
            {
                var products = await _productService.GetProductByIdAsync(id);
                if (products == null)
                {
                    _logger.LogWarning($"Products with Id: {id} not found.");
                    return NotFound();
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the product with Id: {id}");
                return StatusCode(500, "An error ocurred while processing your request.");
            }
        }

        // GET: api/products/by-manufacturer/{manufacturerId}
        [HttpGet("by-manufacturer/{manufacturerId:guid}")]
        public async Task<IActionResult> GetProductByManufacturer(Guid manufacturerId)
        {
            _logger.LogInformation($"Received request to get Product with ID: {manufacturerId}");

            try
            {
                var products = await _productService.GetProductsByManufacturerIdAsync(manufacturerId);
                if(products == null)
                {
                    _logger.LogWarning($"No Products found with ID: {manufacturerId}");
                    return NotFound();
                }

                return Ok(products);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the products with Id: {manufacturerId}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid Inputs
            }

            _logger.LogInformation($"Received request to create a new Product");

            try
            {
                var createdProduct = await _productService.CreateProductAsync(productDto);

                _logger.LogInformation($"Product with ID: {productDto.ProductId} created.");
                return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
            
        }

        //PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProduct(Guid id, CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update Product with ID: {id}");

            try
            {
                var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
                if (productDto == null)
                {
                    return NotFound();  // 400 Not Found is the product doesn't exists.
                }

                _logger.LogInformation($"Product with ID: {id} update successfully.");
                return Ok(updatedProduct); // 200 OK with updated product data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the product with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        
        // DELETE: api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            _logger.LogInformation($"Received request to delete Product with ID: {id}");

            try
            {
                var deleted = await _productService.DeleteProductAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Product with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the product doesn't exist
                }

                _logger.LogInformation($"Product with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the Product with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
