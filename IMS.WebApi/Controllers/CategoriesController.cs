using IMS.Application.DTOs.Categories;
using IMS.Application.Services.CategoryServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private ILogger<CategoriesController> _logger;
        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            _logger.LogInformation("Received request to get all categories.");

            try
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                if(categories == null || !categories.Any())
                {
                    _logger.LogWarning("No categories found.");
                    return NoContent();
                }

                return Ok(categories); // Return 200 OK with category data
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all categories.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/categoru/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            _logger.LogInformation($"Received request to get category with ID: {id}");

            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if(category == null)
                {
                    _logger.LogWarning($"Category with ID: {id} not found.");
                    return NotFound(); // 404 Not Found
                }

                return Ok(category); // Return 200 OK with category data
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the category with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto createCategoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for invalid input
            }

            _logger.LogInformation("Received request to create a new category.");

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(createCategoryDto);

                _logger.LogInformation($"Category with ID: {createCategoryDto.CategoryId} created.");
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
                // 201 Created with location header
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occrred while creating the category.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/Category/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryDto categoryDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update category with ID: {id}");

            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                if(UpdateCategory == null)
                {
                    return NotFound();  // 404 Not FOund is the category Doesn't exists.
                }

                _logger.LogInformation($"Category with ID: {id} updated successfully.");
                return Ok(updatedCategory); // 200 OK with updated category data
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the category with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/Category/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            _logger.LogInformation($"Received request to delete category with ID: {id}");

            try
            {
                var deleted = await _categoryService.DeleteCategoryAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Category with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the category doesn't exist
                }

                _logger.LogInformation($"Category with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the category with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
