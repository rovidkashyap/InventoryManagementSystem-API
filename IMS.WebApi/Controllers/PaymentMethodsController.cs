using IMS.Application.DTOs.PaymentMethods;
using IMS.Application.Services.PaymentMethodServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private ILogger<PaymentMethodsController> _logger;
        public PaymentMethodsController(IPaymentMethodService paymentMethodService, ILogger<PaymentMethodsController> logger)
        {
            _paymentMethodService = paymentMethodService;
            _logger = logger;
        }

        // GET:api/paymentmethods
        [HttpGet]
        public async Task<IActionResult> GetAllPaymentMethods()
        {
            _logger.LogInformation("Received request to get all the PaymentMethods");
            try
            {
                var paymentMethod = await _paymentMethodService.GetPaymentMethodAsync();

                if(paymentMethod == null || !paymentMethod.Any())
                {
                    _logger.LogInformation("No Record Found.");
                    return NoContent();
                }
                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all PaymentMethod");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/paymentmethods/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPaymentMethodById(Guid id)
        {
            _logger.LogInformation($"Received request to get PaymentMethod with Id: {id}");

            try
            {
                var paymentMethod = await _paymentMethodService.GetPaymentMethodByIdAsync(id);
                if (paymentMethod == null)
                {
                    _logger.LogWarning($"PaymentMethod with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(paymentMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the PaymentMethod with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // POST: api/paymentmethods
        [HttpPost]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodDto paymentMethodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid inputs
            }

            _logger.LogInformation($"Receieved request to create a new PaymentMethod");

            try
            {
                var createdPaymentMethod = await _paymentMethodService.CreatePaymentMethodAsync(paymentMethodDto);

                _logger.LogInformation($"PaymentMethod with ID: {paymentMethodDto.PaymentMethodId} created.");
                return CreatedAtAction(nameof(GetPaymentMethodById), new { id = createdPaymentMethod.PaymentMethodId }, createdPaymentMethod);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occrred while creating the PaymentMethod.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/paymentmethods
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePaymentMethod(Guid id, CreatePaymentMethodDto updatePaymentMethodDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update PaymentMethod with ID: {id}");

            try
            {
                var updatedPaymentMethod = await _paymentMethodService.UpdatePaymentMethodAsync(id, updatePaymentMethodDto);
                if (updatePaymentMethodDto == null)
                {
                    return NotFound();  // 400 Not Found is the transaction doesn't exists.
                }

                _logger.LogInformation($"PaymentMethod with ID: {id} update successfully.");
                return Ok(updatedPaymentMethod); // 200 OK with updated PaymentMethod data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the PaymentMethod with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/paymentmethods
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePaymentMethod(Guid id)
        {
            _logger.LogInformation($"Received request to delete PaymentMethod with ID: {id}");

            try
            {
                var deleted = await _paymentMethodService.DeletePaymentMethodAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"PaymentMethod with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the PaymentMethod doesn't exist
                }

                _logger.LogInformation($"PaymentMethod with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the PaymentMethod with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
