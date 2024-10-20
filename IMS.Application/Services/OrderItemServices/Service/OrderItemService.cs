using AutoMapper;
using IMS.Application.Common;
using IMS.Application.Common.Utility;
using IMS.Application.DTOs.OrderItems;
using IMS.Application.Services.OrderItemServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.OrderItemServices.Service
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<OrderItemService> _logger;
        private IMemoryCache _cache;
        public OrderItemService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderItemService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayOrderItemDto>> GetAllOrderItemAsync()
        {
            string cacheKey = CacheKeys.OrderItems;
            try
            {
                _logger.LogInformation("Fetch all orderitem");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayOrderItemDto> result))
                {
                    _logger.LogInformation("Returning cached orderitem.");
                    return result;
                }

                // If not in cache, fetch from the database
                var orderItems = await _unitOfWork.OrderItems.GetAllAsync();
                var orderItemDto = _mapper.Map<IEnumerable<DisplayOrderItemDto>>(orderItems);

                // cache the result
                _cache.Set(cacheKey, orderItemDto, TimeSpan.FromMinutes(5));

                return orderItemDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orderitem.");
                throw;
            }
        }

        public async Task<DisplayOrderItemDto> GetOrderItemByIdAsync(Guid id)
        {
            var cacheKey = CacheKeys.GetOrderItemById(id);
            try
            {
                _logger.LogInformation($"Fetching orderitem with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayOrderItemDto cachedOrderItems))
                {
                    _logger.LogInformation("Returning cached orderitems");
                    return cachedOrderItems;
                }

                // If not in cache, fetch from database
                var orderitems = await _unitOfWork.OrderItems.GetByIdAsync(id);
                if (orderitems == null)
                {
                    _logger.LogWarning($"OrderItems with ID: {id} was not found.");
                    return null;
                }

                var orderItemsDto = _mapper.Map<DisplayOrderItemDto>(orderitems);

                // Cache the result
                _cache.Set(cacheKey, orderItemsDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return orderItemsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the orderitems with ID {id}.");
                throw;
            }
        }

        public async Task<DisplayOrderItemDto> CreateOrderItemAsync(CreateOrderItemDto orderItemDto)
        {
            if(orderItemDto == null)
            {
                _logger.LogWarning("CreateOrderItemAsync called with null orderItemDto");
                throw new ArgumentNullException(nameof(orderItemDto), "Order item data is required.");
            }

            try
            {
                // Map DTO to the OrderItem entity
                var orderitem = _mapper.Map<OrderItem>(orderItemDto);

                //Calculte TotalPrice and NetPrice using the utility method
                //orderitem.TotalPrice = OrderItemCalculator.CalculateTotalPrice(orderitem.Quantity, orderitem.UnitPrice);
                //orderitem.TaxAmount = OrderItemCalculator.CalculateTaxAmount(orderitem.TotalPrice);
                //orderitem.DiscountAmount = OrderItemCalculator.CalculateDiscountAmount(orderitem.TotalPrice, 5);    // Flat 5% Discount
                //orderitem.NetPrice = OrderItemCalculator.CalculateNetPrice(orderitem.TotalPrice, orderitem.DiscountAmount, orderitem.TaxAmount);

                // Add the new orderitem to the repository
                await _unitOfWork.OrderItems.CreateAsync(orderitem);
                await _unitOfWork.CompleteAsync();

                // Map back to the DisplayOrderItemDto
                var orderitemDto = _mapper.Map<DisplayOrderItemDto>(orderitem);

                // Log success
                _logger.LogInformation($"Order Item created successfully with ID: {orderitem.OrderItemId}");

                return orderitemDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order item.");
                throw new ApplicationException("An error occurred while creating the order item", ex);
            }
        }

        public async Task<DisplayOrderItemDto> UpdateOrderItemAsync(Guid id, UpdateOrderItemDto orderItemDto)
        {
            if (orderItemDto == null)
            {
                _logger.LogWarning("UpdateOrderItemAsync called with null orderItemDto.");
                throw new ArgumentNullException(nameof(orderItemDto), "Order item data is required.");
            }

            try
            {
                // Check if the order item exists in cache first
                var cacheKey = CacheKeys.GetOrderById(id);
                if (!_cache.TryGetValue(cacheKey, out OrderItem existingOrderItem))
                {
                    // Fetch from database if not in cache
                    existingOrderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);
                    if (existingOrderItem == null)
                    {
                        _logger.LogWarning($"Order item with ID {id} not found.");
                        throw new KeyNotFoundException("Order item not found.");
                    }
                }

                // Update the existing order item using AutoMapper
                _mapper.Map(orderItemDto, existingOrderItem);

                // Recalculate TotalPrice and NetPrice using the utility methods
                existingOrderItem.TotalPrice = OrderItemCalculator.CalculateTotalPrice(existingOrderItem.Quantity, existingOrderItem.UnitPrice);
                existingOrderItem.NetPrice = OrderItemCalculator.CalculateNetPrice(existingOrderItem.TotalPrice, existingOrderItem.DiscountAmount, existingOrderItem.TaxAmount);

                // Update the repository
                await _unitOfWork.OrderItems.UpdateAsync(existingOrderItem, id);
                await _unitOfWork.CompleteAsync();

                // Remove the old cache entry and set the updated one
                _cache.Remove(cacheKey);
                _cache.Set(cacheKey, existingOrderItem, TimeSpan.FromMinutes(10)); // Cache for 10 minutes

                // Map back to DisplayOrderItemDto
                var updatedOrderItemDto = _mapper.Map<DisplayOrderItemDto>(existingOrderItem);

                // Log success
                _logger.LogInformation($"Order item updated successfully with ID: {existingOrderItem.OrderItemId}");

                return updatedOrderItemDto;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Order item not found.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order item.");
                throw new ApplicationException("An error occurred while updating the order item.", ex);
            }
        }

        public async Task<bool> DeleteOrderItemAsync(Guid id)
        {
            try
            {
                // Fetch the existing orderItem from the unitofwork
                var existingOrderItem = await _unitOfWork.OrderItems.GetByIdAsync(id);

                if (existingOrderItem == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing orderItem with ID: {id}");
                    return false; // Return false if the orderItem was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting orderItem with ID: {id}");

                // Delete the orderItem from the repository
                await _unitOfWork.OrderItems.DeleteAsync(id);

                // Invalidate caches for this orderItem and all orderItems
                _cache.Remove(CacheKeys.GetOrderItemById(id));
                _cache.Remove(CacheKeys.OrderItems);

                // Log successful orderItem deletion
                _logger.LogInformation($"OrderItem with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the orderItem with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the orderItem", ex);
            }
        }
    }
}
