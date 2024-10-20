using IMS.Application.DTOs.Suppliers;
using IMS.Application.Services.SupplierServices.Interface;
using IMS.Application.Services.SupplierServices.Service;
using IMS.Core.Entities;
using IMS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private ILogger<SuppliersController> _logger;
        public SuppliersController(ISupplierService supplierService, ILogger<SuppliersController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        //GET: api/suppliers/GetAllSuppliersCount
        [HttpGet("GetAllSuppliersCount")]
        public async Task<IActionResult> GetAllSuppliersCount()
        {
            try
            {
                var count = await _supplierService.GetAllSuppliersCountAsync();
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "Products count retrieved successfully."
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting suppliers count.");

                var errorResponse = new ApiResponse<object>
                {
                    Data = 0,
                    Success = false,
                    Message = "An error occurred while processing your request."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        // GET: api/suppliers
        [HttpGet]
        public async Task<IActionResult> GetAllSuppliers()
        {
            _logger.LogInformation("Received request to get all Supplier.");
            try
            {
                var suppliers = await _supplierService.GetAllSuppliersAsync();

                if (suppliers == null || !suppliers.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all Supplier.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET:api/suppliers/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            _logger.LogInformation($"Received request to get Supplier with Id: {id}");

            try
            {
                var suppliers = await _supplierService.GetSupplierByIdAsync(id);
                if (suppliers == null)
                {
                    _logger.LogWarning($"Supplier with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the Supplier with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //POST: api/suppliers
        [HttpPost]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto suppliersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid inputs
            }

            _logger.LogInformation($"Receieved request to create a new Supplier");

            try
            {
                var createdSupplier = await _supplierService.CreateSupplierAsync(suppliersDto);

                _logger.LogInformation($"Supplier with ID: {suppliersDto.SupplierId} created.");
                return CreatedAtAction(nameof(GetSupplierById), new { id = createdSupplier.SupplierId }, createdSupplier);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occrred while creating the Supplier.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //PUT: api/suppliers/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateSupplier(Guid id, CreateSupplierDto suppliersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update Supplier with ID: {id}");

            try
            {
                var updatedSupplier = await _supplierService.UpdateSupplierAsync(id, suppliersDto);
                if (suppliersDto == null)
                {
                    return NotFound();  // 400 Not Found is the Supplier doesn't exists.
                }

                _logger.LogInformation($"Supplier with ID: {id} update successfully.");
                return Ok(updatedSupplier); // 200 OK with updated Supplier data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the Supplier with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/suppliers/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteSupplier(Guid id)
        {
            _logger.LogInformation($"Received request to delete Supplier with ID: {id}");

            try
            {
                var deleted = await _supplierService.DeleteSupplierAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Supplier with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the suppliers doesn't exist
                }

                _logger.LogInformation($"Supplier with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the Supplier with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
