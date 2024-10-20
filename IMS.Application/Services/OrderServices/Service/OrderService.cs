using AutoMapper;
using IMS.Application.Common;
using IMS.Application.Common.Utility;
using IMS.Application.DTOs.Inventories;
using IMS.Application.DTOs.OrderItems;
using IMS.Application.DTOs.Orders;
using IMS.Application.Services.OrderServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.Repositories.OrderRepository.Repository;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace IMS.Application.Services.OrderServices.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitofWork;
        private readonly IMapper _mapper;
        private ILogger<OrderService> _logger;
        private IMemoryCache _cache;
        public OrderService(IUnitOfWork unitofWork, IMapper mapper, ILogger<OrderService> logger,
            IMemoryCache cache)
        {
            _unitofWork = unitofWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> GetAllOrdersCountAsync()
        {
            return await _unitofWork.Orders.GetAllOrdersCountAsync();
        }

        public async Task<IEnumerable<DisplayOrderDto>> GetAllOrderAsync()
        {
            string cacheKey = CacheKeys.Order;
            try
            {
                _logger.LogInformation("Fetch all orders");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayOrderDto> result))
                {
                    _logger.LogInformation("Returning cached order.");
                    return result;
                }

                // If not in cache, fetch from the database
                var orders = await _unitofWork.Orders.GetAllAsync();
                var orderDto = _mapper.Map<IEnumerable<DisplayOrderDto>>(orders);

                // cache the result
                _cache.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5));

                return orderDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orders.");
                throw;
            }
        }

        public async Task<DisplayOrderDto> GetOrderByIdAsync(Guid id)
        {
            var cacheKey = CacheKeys.GetOrderItemById(id);
            try
            {
                _logger.LogInformation($"Fetching order with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayOrderDto cachedOrder))
                {
                    _logger.LogInformation("Returning cached orders");
                    return cachedOrder;
                }

                // If not in cache, fetch from database
                var order = await _unitofWork.Orders.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning($"Order with ID: {id} was not found.");
                    return null;
                }

                var orderDto = _mapper.Map<DisplayOrderDto>(order);

                // Cache the result
                _cache.Set(cacheKey, orderDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return orderDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the order with ID {id}.");
                throw;
            }
        }

        public async Task<DisplayOrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            if(createOrderDto == null)
            {
                _logger.LogWarning("CreateOrderAsync called with null orderDto.");
                throw new ArgumentNullException(nameof(createOrderDto), "Order data is required.");
            }

            try
            {
                // Map the DTO to entity
                var order = _mapper.Map<Order>(createOrderDto);

                // Calculated TotalPrice and NetAmount
                var totalPrice = OrderCalculator.CalculateTotalPrice(order.OrderItems);
                var taxAmount = OrderCalculator.CalculateTaxAmount(order.TotalPrice);
                var discountAmount = OrderCalculator.CalculateDiscountAmount();
                var shippingCost = OrderCalculator.CalculateShippingCost(order.TotalPrice);
                var netAmount = OrderCalculator.CalculateNetAmount(order.TotalPrice, order.TaxAmount, order.DiscountAmount, order.ShippingCost);

                // Set calculated field on Order
                order.TotalPrice = totalPrice;
                order.TaxAmount = taxAmount;
                order.DiscountAmount = discountAmount;
                order.ShippingCost = shippingCost;
                order.NetAmount = netAmount;

                // Add Order to the repository
                await _unitofWork.Orders.CreateAsync(order);

                // Map OrderItems and add them to the respository
                foreach (var itemDto in createOrderDto.OrderItems)
                {
                    var orderItem = _mapper.Map<OrderItem>(itemDto);

                    // Calculated Automatic fields for orderItem
                    var totalPriceItem = OrderItemCalculator.CalculateTotalPrice(orderItem.Quantity, orderItem.UnitPrice);
                    var taxAmountItem = OrderItemCalculator.CalculateTaxAmount(totalPriceItem);
                    var discountAmountItem = OrderItemCalculator.CalculateDiscountAmount(totalPriceItem, itemDto.DiscountAmount);
                    var netPriceItem = OrderItemCalculator.CalculateNetPrice(totalPriceItem, discountAmountItem, taxAmountItem);

                    // Set Calculated fields on OrderItem
                    orderItem.TotalPrice = totalPriceItem;
                    orderItem.TaxAmount = taxAmountItem;
                    orderItem.DiscountAmount = discountAmountItem;
                    orderItem.NetPrice = netPriceItem;

                    await _unitofWork.OrderItems.CreateAsync(orderItem);
                }

                // Commit the transaction
                await _unitofWork.CompleteAsync();

                // Map back to the DisplayOrderDto
                var orderDto = _mapper.Map<DisplayOrderDto>(order);

                // Cache the created order
                _cache.Set(order.OrderId, orderDto);

                // Log success
                _logger.LogInformation($"Order created successfully with ID: {order.OrderId}");

                return orderDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order.");
                throw new ApplicationException("An error occurred while creating the order.", ex);
            }
            
        }

        public async Task<DisplayOrderDto> UpdateOrderAsync(Guid id, UpdateOrderDto updateOrderDto)
        {
            if (updateOrderDto == null)
            {
                _logger.LogWarning("UpdateOrderAsync called with null UpdateOrderDto.");
                throw new ArgumentNullException(nameof(updateOrderDto), "Order data is required.");
            }

            try
            {
                var existingOrder = await _unitofWork.Orders.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    _logger.LogWarning($"Order with ID {id} not found.");
                    throw new KeyNotFoundException("Order not found.");
                }

                // Update the existing order using AutoMapper
                _mapper.Map(updateOrderDto, existingOrder);

                // Recalculate TotalPrice and NetAmount
                existingOrder.TotalPrice = OrderCalculator.CalculateTotalPrice(existingOrder.OrderItems);
                existingOrder.TaxAmount = OrderCalculator.CalculateTaxAmount(existingOrder.TotalPrice);
                existingOrder.DiscountAmount = OrderCalculator.CalculateDiscountAmount();
                existingOrder.ShippingCost = OrderCalculator.CalculateShippingCost(existingOrder.TotalPrice);
                existingOrder.NetAmount = OrderCalculator.CalculateNetAmount(existingOrder.TotalPrice, existingOrder.TaxAmount, existingOrder.DiscountAmount, existingOrder.ShippingCost);

                // Update the repository
                await _unitofWork.Orders.UpdateAsync(existingOrder, id);
                await _unitofWork.CompleteAsync();

                // Map back to DisplayOrderDto
                var updatedOrderDto = _mapper.Map<DisplayOrderDto>(existingOrder);

                // Log success
                _logger.LogInformation($"Order updated successfully with ID: {existingOrder.OrderId}");

                return updatedOrderDto;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order.");
                throw new ApplicationException("An error occurred while updating the order.", ex);
            }
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            try
            {
                // Fetch the existing order from the unitofwork
                var existingOrder = await _unitofWork.Orders.GetByIdAsync(id);

                if (existingOrder == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing order with ID: {id}");
                    return false; // Return false if the order was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting order with ID: {id}");

                // Delete the order from the repository
                await _unitofWork.Orders.DeleteAsync(id);

                // Invalidate caches for this order and all orders
                _cache.Remove(CacheKeys.GetOrderItemById(id));
                _cache.Remove(CacheKeys.Order);

                // Log successful order deletion
                _logger.LogInformation($"Order with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the order with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the order", ex);
            }
        }

        public async Task<IEnumerable<DisplayOrdersForOrderItemsDto>> GetAllOrdersWithOrderItemsAsync()
        {
            var orders = await _unitofWork.Orders.GetAllOrdersWithOrderItemsAsync();

            // Map Orders to DTO
            return orders.Select(order => new DisplayOrdersForOrderItemsDto
            {
                OrderId = order.OrderId,
                CustomerName = order.Customer?.CustomerName ?? "Unknown Customer Name",
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                TaxAmount = order.TaxAmount,
                DiscountAmount = order.DiscountAmount,
                ShippingCost = order.ShippingCost,
                NetAmount = order.NetAmount,
                OrderStatusName = order.OrderStatus.OrderStatusName ?? "Unknown Order Status",
                PaymentMethodName = order.PaymentMethod?.PaymentMethodName ?? "Unknown Payment Method",
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                BillingEmail = order.BillingEmail,
                BillingPhone = order.BillingPhone,
                ShippingDate = order.ShippingDate,
                DeliveredDate = order.DeliveredDate,
                Notes = order.Notes,
                OrderItems = order.OrderItems?.Select(item => new DisplayOrderItemsForOrdersDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductName = item.Product?.ProductName ?? "Unkonwn Product",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TotalPrice = item.TotalPrice,
                    DiscountAmount = item.DiscountAmount,
                    TaxAmount = item.TaxAmount,
                    NetPrice = item.NetPrice
                }).ToList() ?? new List<DisplayOrderItemsForOrdersDto>()
            }).ToList();
        }

        public async Task<int> GetOrdersCountByOrderStatus(string statusName)
        {
            var count = await _unitofWork.Orders.GetOrdersCountByOrderStatus(statusName);
            return count;
        }

        public async Task<List<OrderDto>> GetRecentOrdersAsync(int count)
        {
            var orders = await _unitofWork.Orders.GetRecentOrdersAsync(count);

            var ordersDto = _mapper.Map<List<OrderDto>>(orders);

            return ordersDto;
        }
    }
}
