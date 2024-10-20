using IMS.Application.DTOs.Orders;
using IMS.Application.Services.OrderServices.Interface;
using IMS.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET: api/orders/GetAllOrdersWithOrderItems
        [HttpGet("GetAllOrdersWithOrderItems")]
        public async Task<IActionResult> GetAllOrdersWithOrderItems()
        {
            var orders = await _orderService.GetAllOrdersWithOrderItemsAsync();

            if(orders == null || !orders.Any())
            {
                return NotFound("No Order found.");
            }

            return Ok(orders);
        }

        // GET: api/orders/GetAllOrdersCount
        [HttpGet("GetAllOrdersCount")]
        public async Task<IActionResult> GetAllOrdersCount()
        {
            try
            {
                var count = await _orderService.GetAllOrdersCountAsync();
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "An error occurred while getting orders count."
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred while getting orders count.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // GET: api/orders/GetRecentOrders
        [HttpGet("GetRecentOrders")]
        public async Task<IActionResult> GetRecentOrders(int count)
        {
            if(count <= 0)
            {
                _logger.LogWarning($"Invalid count parameter: {count}. Must be greater than zero.");
                return BadRequest("Count must be greater than zero.");
            }

            try
            {
                _logger.LogInformation($"Request to get {count} recent orders received.");
                var recentOrders = await _orderService.GetRecentOrdersAsync(count);

                _logger.LogInformation($"Successfully retrieved {recentOrders.Count} recent orders.");
                return Ok(recentOrders);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving recent orders.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal Server error.");
            }
        }

        // GET: api/orders/GetOrdersCountByStatusName
        [HttpGet("GetOrdersCountByStatusName")]
        public async Task<IActionResult> GetOrdersCountByStatusName(string statusName)
        {
            try
            {
                var count = await _orderService.GetOrdersCountByOrderStatus(statusName);
                var response = new ApiResponse<int>
                {
                    Data = count,
                    Success = true,
                    Message = "An error occurred while getting orders count by statusname."
                };

                return Ok(response);
            }
            catch(Exception ex)
            {
                _logger.LogError("An error occurred while getting orders count bby statusname.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        //GET: api/orders
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            _logger.LogInformation("Recieved request to get all orders.");
            try
            {
                var order = await _orderService.GetAllOrderAsync();

                if(order == null || !order.Any())
                {
                    _logger.LogInformation("No Record Found.");
                    return NoContent();
                }

                return Ok(order);
            }
            catch(Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all orders.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            _logger.LogInformation($"Recieved request to get order with Id: {id}");

            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning($"Order with Id: {id} not found.");
                    return NotFound();
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the order with Id: {id}");
                return StatusCode(500, "An error ocurred while processing your request.");
            }
        }

        // POST: api/orders
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            if (createOrderDto == null)
            {
                _logger.LogWarning("CreateOrder called with null CreateOrderDto.");
                return BadRequest("Order data is required.");
            }

            try
            {
                var result = await _orderService.CreateOrderAsync(createOrderDto);
                _logger.LogInformation($"Order created successfully with ID: {result.OrderId}");

                return CreatedAtAction(nameof(GetOrderById), new { id = result.OrderId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // PUT: api/orders/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            if (updateOrderDto == null)
            {
                _logger.LogWarning("UpdateOrder called with null UpdateOrderDto.");
                return BadRequest("Order data is required.");
            }

            if (id != updateOrderDto.OrderId)
            {
                _logger.LogWarning("Mismatched ID: ID in URL does not match ID in body.");
                return BadRequest("Mismatched ID.");
            }

            try
            {
                var updatedOrder = await _orderService.UpdateOrderAsync(id, updateOrderDto);
                _logger.LogInformation($"Order updated successfully with ID: {updatedOrder.OrderId}");
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found.");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/orders/{id}
        [HttpDelete]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            _logger.LogInformation($"Received request to delete order with ID: {id}");

            try
            {
                var deleted = await _orderService.DeleteOrderAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Order with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the order doesn't exist
                }

                _logger.LogInformation($"Order with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the order with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
