using IMS.Application.DTOs.Customers;
using IMS.Application.DTOs.Inventories;
using IMS.Application.Services.InventoryServices.Interface;
using IMS.Application.Services.InventoryServices.Service;
using IMS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private ILogger<InventoriesController> _logger;
        public InventoriesController(IInventoryService inventoryService, ILogger<InventoriesController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        //GET: api/inventories/GetAllInventoryCount
        [HttpGet("GetAllInventoryCount")]
        public async Task<IActionResult> GetAllInventoryCount()
        {
            try
            {
                var count = await _inventoryService.GetAllInventoryCountAsync();
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "An error occurred while getting inventory count."
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred while getting inventory count.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        //GET: api/inventories
        [HttpGet]
        public async Task<IActionResult> GetAllinventories()
        {
            _logger.LogInformation("Received request to get all inventories.");
            try
            {
                var inventory = await _inventoryService.GetAllInventoriesAsync();

                if (inventory == null || !inventory.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all inventory.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/inventories/GetRecentInventories
        [HttpGet("GetRecentInventories")]
        public async Task<IActionResult> GetRecentInventories()
        {
            _logger.LogInformation("Received request to get recent inventories.");
            try
            {
                var inventory = await _inventoryService.GetRecentInventories();
                if (inventory == null || !inventory.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }

                return Ok(inventory);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting recent inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurerd while processing your request.");
            }
        }

        // GET: api/inventories/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetInventoryById(Guid id)
        {
            _logger.LogInformation($"Recieved request to get inventory with Id: {id}");

            try
            {
                var inventory = await _inventoryService.GetInventoryByIdAsync(id);
                if (inventory == null)
                {
                    _logger.LogWarning($"inventory with Id: {id} not found.");
                    return NotFound();
                }

                return Ok(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the inventory with Id: {id}");
                return StatusCode(500, "An error ocurred while processing your request.");
            }
        }

        // POST: api/inventories
        [HttpPost]
        public async Task<IActionResult> CreateInventory([FromBody] CreateInventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid Inputs
            }

            _logger.LogInformation($"Received request to create a new inventory");

            try
            {
                var createdInventory = await _inventoryService.CreateInventoryAsync(inventoryDto);

                _logger.LogInformation($"Inventory with ID: {inventoryDto.InventoryId} created.");
                return CreatedAtAction(nameof(GetInventoryById), new { id = createdInventory.InventoryId }, createdInventory);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the inventory.");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        //PUT: api/inventories/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateInventory(Guid id, CreateInventoryDto inventoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update inventory with ID: {id}");

            try
            {
                var updatedInventory = await _inventoryService.UpdateInventoryAsync(id, inventoryDto);
                if (inventoryDto == null)
                {
                    return NotFound();  // 400 Not Found is the inventory doesn't exists.
                }

                _logger.LogInformation($"Inventory with ID: {id} update successfully.");
                return Ok(updatedInventory); // 200 OK with updated inventory data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the inventory with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/inventories/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteInventory(Guid id)
        {
            _logger.LogInformation($"Received request to delete inventory with ID: {id}");

            try
            {
                var deleted = await _inventoryService.DeleteInventoryAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Inventory with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the inventory doesn't exist
                }

                _logger.LogInformation($"Inventory with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the inventory with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
