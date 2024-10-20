using IMS.Application.DTOs.OrderStatus;
using IMS.Application.Services.OrderStatusServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService _orderStatusService;
        private ILogger<OrderStatusController> _logger;
        public OrderStatusController(IOrderStatusService orderStatus, ILogger<OrderStatusController> logger)
        {
            _orderStatusService = orderStatus;
            _logger = logger;
        }

        // GET: api/orderstatus
        [HttpGet]
        public async Task<IActionResult> GetAllOrderStatus()
        {
            var orderstatus = await _orderStatusService.GetAllOrderStatusAsync();

            if(orderstatus == null || !orderstatus.Any())
            {
                return NotFound("No Order found.");
            }

            return Ok(orderstatus);
        }

        // GET: api/orderstatus/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderStatusById(Guid id)
        {
            try
            {
                var orderStatus = await _orderStatusService.GetOrderStatusByIdAsync(id);
                _logger.LogInformation($"Order status retrieved successfully with ID: {id}");
                return Ok(orderStatus);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order status not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the order status.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // POST: api/orderstatus
        [HttpPost]
        public async Task<IActionResult> CreateOrderStatus([FromBody] CreateOrderStatusDto orderStatusDto)
        {
            if(orderStatusDto == null)
            {
                _logger.LogWarning("CreateOrderStatus called with null DTO.");
                return BadRequest("Order Status data is required.");
            }

            try
            {
                var result = await _orderStatusService.CreateOrderStatusAsync(orderStatusDto);
                _logger.LogInformation($"Order status created successfully with ID: {result.OrderStatusId}");

                return CreatedAtAction(nameof(GetOrderStatusById), new { id = result.OrderStatusId }, result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the orderstatus.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // PUT: api/orderstatus/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            if (updateOrderStatusDto == null)
            {
                _logger.LogWarning("UpdateOrderStatus called with null UpdateOrderStatusDto.");
                return BadRequest("Order status data is required.");
            }

            if (id != updateOrderStatusDto.OrderStatusId)
            {
                _logger.LogWarning("Mismatched ID: ID in URL does not match ID in body.");
                return BadRequest("Mismatched ID.");
            }

            try
            {
                var updatedOrderStatus = await _orderStatusService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
                _logger.LogInformation($"Order status updated successfully with ID: {updatedOrderStatus.OrderStatusId}");
                return Ok(updatedOrderStatus);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order status not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order status.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/orderstatus
        [HttpDelete]
        public async Task<IActionResult> DeleteOrderStatus(Guid orderStatusId)
        {
            try
            {
                var result = await _orderStatusService.DeleteOrderStatusAsync(orderStatusId);
                if(result)
                {
                    _logger.LogInformation($"Order Status with ID: {orderStatusId} deleted successfully.");
                    return Ok(new { message = "Order status deleted successfully." });
                }

                _logger.LogError($"Order Status with ID: {orderStatusId} was not found.");
                return NotFound(new { message = "Order status not found." });
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
