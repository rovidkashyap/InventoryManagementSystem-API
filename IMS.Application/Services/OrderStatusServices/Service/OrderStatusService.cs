using AutoMapper;
using IMS.Application.Common;
using IMS.Application.DTOs.OrderStatus;
using IMS.Application.Services.OrderStatusServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.OrderStatusServices.Service
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderStatusService> _logger;
        private readonly IMemoryCache _cache;
        public OrderStatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderStatusService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayOrderStatusDto>> GetAllOrderStatusAsync()
        {
            string cacheKey = CacheKeys.OrderStatus;
            try
            {
                _logger.LogInformation("Fetch all order statuses");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayOrderStatusDto> cachedOrderStatus))
                {
                    _logger.LogInformation("Returning cached order staus.");
                    return cachedOrderStatus;
                }

                // If not in cache, fetch from the database
                var orderstatus = await _unitOfWork.OrderStatus.GetAllAsync();
                var orderstatusDto = _mapper.Map<IEnumerable<DisplayOrderStatusDto>>(orderstatus);

                // cache the result
                _cache.Set(cacheKey, orderstatusDto, TimeSpan.FromMinutes(5));

                return orderstatusDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orderstatus");
                throw;
            }

        }

        public async Task<DisplayOrderStatusDto> GetOrderStatusByIdAsync(Guid id)
        {
            var cacheKey = CacheKeys.GetOrderItemById(id);
            try
            {
                _logger.LogInformation($"Fetching orderstatus with ID: {id}");

                // Check if the data is in cache
                if(_cache.TryGetValue(cacheKey, out DisplayOrderStatusDto cachedOrderStatus))
                {
                    _logger.LogInformation("Returning cached orderStatus.");
                    return cachedOrderStatus;
                }

                // If not in cache, fetch from database
                var orderStatus = await _unitOfWork.OrderStatus.GetByIdAsync(id);
                if(orderStatus == null)
                {
                    _logger.LogWarning($"OrderStatus with ID: {id} was not found.");
                    return null;
                }

                var orderStatusDto = _mapper.Map<DisplayOrderStatusDto>(orderStatus);

                // Cache the result
                _cache.Set(cacheKey, orderStatusDto, TimeSpan.FromMinutes(5));

                return orderStatusDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the order status with ID {id}");
                throw;
            }
        }

        public async Task<DisplayOrderStatusDto> CreateOrderStatusAsync(CreateOrderStatusDto orderStatusDto)
        {
            if(orderStatusDto == null)
            {
                throw new ArgumentNullException(nameof(orderStatusDto), "The orderStatus DTO cannot be null.");
            }

            try
            {
                // Check if the status name already exists to prevent duplicates
                var exisitingOrderStatus = await _unitOfWork.OrderStatus
                    .FindAsync(o => o.OrderStatusName == orderStatusDto.OrderStatusName);

                if(exisitingOrderStatus != null)
                {
                    _logger.LogWarning($"Order status '{orderStatusDto.OrderStatusName}' already exists");
                    throw new InvalidOperationException($"Order Status '{orderStatusDto.OrderStatusName}' already exists.");
                }

                // Map the CreateOrderStatusDto to 
                var newOrderStatus = _mapper.Map<OrderStatus>(orderStatusDto);

                // Add the new OrdeStatus to the repository
                await _unitOfWork.OrderStatus.CreateAsync(newOrderStatus);

                //Commit the transaction
                await _unitOfWork.CompleteAsync();

                // Log the success
                _logger.LogInformation($"Order Status: '{newOrderStatus.OrderStatusName}' created successfully.");

                // Use AutoAmpper to map the created orderStatus entity to DTO for return
                return _mapper.Map<DisplayOrderStatusDto>(newOrderStatus);
            }
            catch(InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "A business validation error occurred while creating the order status.");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the order status.");
                throw new ApplicationException("An error occurred while creating the order status. Please try again later.", ex);
            }
        }

        public async Task<DisplayOrderStatusDto> UpdateOrderStatusAsync(Guid id, UpdateOrderStatusDto orderStatusDto)
        {
            if(orderStatusDto == null)
            {
                _logger.LogError("UpdateOrderStatusAsync called with Null DTO.");
                throw new ArgumentException(nameof(orderStatusDto));
            }

            try
            {
                // Fetch the existing orderstatus entity by id
                var existingOrderStatus = await _unitOfWork.OrderStatus.GetByIdAsync(id);

                if(existingOrderStatus == null)
                {
                    _logger.LogWarning($"Order status with ID: '{orderStatusDto.OrderStatusId}' not found.");
                    throw new KeyNotFoundException($"Order status with ID '{orderStatusDto.OrderStatusId}' not found.");
                }

                // Update the entity using Automapper
                _mapper.Map(orderStatusDto, existingOrderStatus);

                // Save Changes to database
                await _unitOfWork.OrderStatus.UpdateAsync(existingOrderStatus, id);
                await _unitOfWork.CompleteAsync();

                // Log the success and return the updated DTO
                _logger.LogInformation($"Order status '{existingOrderStatus.OrderStatusName}' updated successfully.");

                // Invalidate caches for this orderstatus
                _cache.Remove(CacheKeys.GetOrderStatusById(id));
                _cache.Remove(CacheKeys.OrderStatus);

                // Map the updated entity back to DTO
                return _mapper.Map<DisplayOrderStatusDto>(existingOrderStatus);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the order status.");
                throw new ApplicationException("An error occurred while updating the order status. Please try again later.", ex);
            }
        }

        public async Task<bool> DeleteOrderStatusAsync(Guid orderStatusId)
        {
            try
            {
                // try to retrieve order status from the cache
                var cacheKey = CacheKeys.GetOrderStatusById(orderStatusId);
                if(!_cache.TryGetValue(cacheKey, out OrderStatus existingOrderStatus))
                {
                    existingOrderStatus = await _unitOfWork.OrderStatus.GetByIdAsync(orderStatusId);
                    if(existingOrderStatus == null)
                    {
                        _logger.LogWarning($"Order status with ID '{orderStatusId}' not found.");
                        throw new KeyNotFoundException($"Order Status with ID '{orderStatusId}' not found.");
                    }
                }

                // Remove the order status from the respository
                await _unitOfWork.OrderStatus.DeleteAsync(orderStatusId);
                await _unitOfWork.CompleteAsync();

                // Invalidate the Cache Entry
                _cache.Remove(CacheKeys.GetOrderStatusById(orderStatusId));
                _cache.Remove(CacheKeys.OrderStatus);

                // Log success
                _logger.LogInformation($"Order Status with ID '{orderStatusId}' was deleted successfully.");

                return true;
            }
            catch(KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, $"Order Status with ID '{orderStatusId} not found.'");
                throw;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the order status with ID '{orderStatusId}'");
                throw new ApplicationException($"An error occurred while deleting the order status. Please try again later.", ex);
            }
        }
    }
}
