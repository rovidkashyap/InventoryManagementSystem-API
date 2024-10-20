using IMS.Application.DTOs.Customers;
using IMS.Application.Services.CustomerServices.Interface;
using IMS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private ILogger<CustomersController> _logger;
        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // GET: api/customers/GetAllCustomersCount
        [HttpGet("GetAllCustomersCount")]
        public async Task<IActionResult> GetAllCustomersCount()
        {
            try
            {
                var count = await _customerService.GetAllCustomersCountAsync();
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "Customer count retrieved successfully."
                };
                return Ok(response);
            }
            catch(Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the customer count request.");

                var errorResponse = new ApiResponse<object>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred while processing your request."
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
        }

        //GET: api/customers
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            _logger.LogInformation("Received request to get all customers.");
            try
            {
                var customers = await _customerService.GetAllCustomerAsync();

                if (customers == null || !customers.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all customers.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/customers/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            _logger.LogInformation($"Recieved request to get customer with Id: {id}");

            try
            {
                var customers = await _customerService.GetCustomerByIdAsync(id);
                if (customers == null)
                {
                    _logger.LogWarning($"Customers with Id: {id} not found.");
                    return NotFound();
                }

                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the customer with Id: {id}");
                return StatusCode(500, "An error ocurred while processing your request.");
            }
        }

        // POST: api/customers
        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid Inputs
            }

            _logger.LogInformation($"Received request to create a new customer");

            try
            {
                var createdCustomer = await _customerService.CreateCustomerAsync(customerDto);

                return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.CustomerId }, createdCustomer);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the customer.");
                return StatusCode(500, "An error occurred while processing your request.");
            }

        }

        //PUT: api/customers/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, CreateCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update Customer with ID: {id}");

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, customerDto);
                if (customerDto == null)
                {
                    return NotFound();  // 400 Not Found is the customer doesn't exists.
                }

                _logger.LogInformation($"Customer with ID: {id} update successfully.");
                return Ok(updatedCustomer); // 200 OK with updated customer data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the customer with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/customers/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            _logger.LogInformation($"Received request to delete customer with ID: {id}");

            try
            {
                var deleted = await _customerService.DeleteCustomer(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Customer with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the customer doesn't exist
                }

                _logger.LogInformation($"Customer with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the Customer with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
