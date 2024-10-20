using IMS.Application.DTOs.OrderItems;
using IMS.Application.Services.OrderItemServices.Interface;
using IMS.Application.Services.OrderItemServices.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;
        private ILogger<OrderItemsController> _logger;
        public OrderItemsController(IOrderItemService orderItemsService, ILogger<OrderItemsController> logger)
        {
            _orderItemService = orderItemsService;
            _logger = logger;
        }

        //GET: api/orderItems
        [HttpGet]
        public async Task<IActionResult> GetAllOrderItems()
        {
            _logger.LogInformation("Recieved request to get all orderItems.");
            try
            {
                var orderItems = await _orderItemService.GetAllOrderItemAsync();

                if (orderItems == null || !orderItems.Any())
                {
                    _logger.LogInformation("No Record Found.");
                    return NoContent();
                }

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all orderItems.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/orderItems/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderItemById(Guid id)
        {
            _logger.LogInformation($"Recieved request to get orderItems with Id: {id}");

            try
            {
                var orderItems = await _orderItemService.GetOrderItemByIdAsync(id);
                if (orderItems == null)
                {
                    _logger.LogWarning($"OrderItems with Id: {id} not found.");
                    return NotFound();
                }

                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the orderItems with Id: {id}");
                return StatusCode(500, "An error ocurred while processing your request.");
            }
        }

        // POST: api/orderItems
        [HttpPost]
        public async Task<IActionResult> CreateOrderItem([FromBody] CreateOrderItemDto orderItemDto)
        {
            if(orderItemDto == null)
            {
                _logger.LogWarning("CreateOrderItem called with null orderItemDTO");
                return BadRequest("Order item data is required.");
            }

            try
            {
                var result = await _orderItemService.CreateOrderItemAsync(orderItemDto);
                _logger.LogInformation($"Order item created successfully with ID: {result.OrderItemId}");

                return CreatedAtAction(nameof(GetOrderItemById), new { id = result.OrderItemId });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order item.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // PUT: api/orderItems/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(Guid id, [FromBody] UpdateOrderItemDto updateOrderItemDto)
        {
            if (updateOrderItemDto == null)
            {
                _logger.LogWarning("UpdateOrderItem called with null UpdateOrderItemDto.");
                return BadRequest("Order item data is required.");
            }

            if (id != updateOrderItemDto.OrderItemId)
            {
                _logger.LogWarning("Mismatched ID: ID in URL does not match ID in body.");
                return BadRequest("Mismatched ID.");
            }

            try
            {
                var updatedOrderItem = await _orderItemService.UpdateOrderItemAsync(id, updateOrderItemDto);
                _logger.LogInformation($"Order item updated successfully with ID: {updatedOrderItem.OrderItemId}");
                return Ok(updatedOrderItem);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order item not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order item.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/orderItems/{id}
        [HttpDelete]
        public async Task<IActionResult> DeleteOrderItem(Guid id)
        {
            _logger.LogInformation($"Received request to delete orderItems with ID: {id}");

            try
            {
                var deleted = await _orderItemService.DeleteOrderItemAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"OrderItem with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the orderItem doesn't exist
                }

                _logger.LogInformation($"OrderItem with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the orderItem with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
