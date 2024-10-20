using IMS.Application.DTOs.Manufacturers;
using IMS.Application.Services.ManufacturerServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManufacturersController : ControllerBase
    {
        private readonly IManufacturerService _manufacturerService;
        private ILogger<ManufacturersController> _logger;
        public ManufacturersController(IManufacturerService manufacturerService, ILogger<ManufacturersController> logger)
        {
            _manufacturerService = manufacturerService;
            _logger = logger;
        }

        // GET: api/manufacturers
        [HttpGet]
        public async Task<IActionResult> GetAllManufacturers()
        {
            _logger.LogInformation("Received request to get all manufacturers.");
            try
            {
                var manufacturer = await _manufacturerService.GetAllManufacturersAsync();

                if (manufacturer == null || !manufacturer.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }
                return Ok(manufacturer);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all manufacturers.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET:api/manufacturers/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetManufacturerById(Guid id)
        {
            _logger.LogInformation($"Received request to get manufacturer with Id: {id}");

            try
            {
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
                if(manufacturer == null)
                {
                    _logger.LogWarning($"Manufacturer with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(manufacturer);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the Manufacturer with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }   
        }

        //POST: api/manufacturer
        [HttpPost]
        public async Task<IActionResult> CreateManufacturer([FromBody] CreateManufacturerDto manufacturerDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid inputs
            }

            _logger.LogInformation($"Receieved request to create a new manufacturer");
            
            try
            {
                var createdManufacturer = await _manufacturerService.CreateManufacturerAsync(manufacturerDto);

                _logger.LogInformation($"Manufacturer with ID: {manufacturerDto.ManufacturerId} created.");
                return CreatedAtAction(nameof(GetManufacturerById), new { id = createdManufacturer.ManufacturerId }, createdManufacturer);
                // 201 created with location header
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occrred while creating the manufacturer.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //PUT: api/manufacturer
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateManufacturer(Guid id, CreateManufacturerDto manufacturerDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update manufacturer with ID: {id}");

            try
            {
                var updatedManufacturer = await _manufacturerService.UpdateManufacturerAsync(id, manufacturerDto);
                if(manufacturerDto == null)
                {
                    return NotFound();  // 400 Not Found is the Manufacturer doesn't exists.
                }

                _logger.LogInformation($"Manufacturer with ID: {id} update successfully.");
                return Ok(updatedManufacturer); // 200 OK with updated manufacturer data
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"An error occurred while updating the category with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/manufacturer
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteManufacturer(Guid id)
        {
            _logger.LogInformation($"Received request to delete manufacturer with ID: {id}");

            try
            {
                var deleted = await _manufacturerService.DeleteManufacturerAsync(id);
                if(!deleted)
                {
                    _logger.LogWarning($"Manufacturer with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the manufacturer doesn't exist
                }

                _logger.LogInformation($"Manufacturer with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the manufacturer with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
