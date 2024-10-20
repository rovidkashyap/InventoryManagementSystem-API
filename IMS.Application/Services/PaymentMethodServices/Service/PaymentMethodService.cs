using AutoMapper;
using IMS.Application.DTOs.Manufacturers;
using IMS.Application.DTOs.PaymentMethods;
using IMS.Application.Services.PaymentMethodServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.PaymentMethodServices.Service
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<PaymentMethodService> _logger;
        private readonly IMemoryCache _cache;
        public PaymentMethodService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache cache,
            ILogger<PaymentMethodService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayPaymentMethodDto>> GetPaymentMethodAsync()
        {
            const string cacheKey = "all_paymentmethod";
            try
            {
                if(_cache.TryGetValue(cacheKey, out IEnumerable<DisplayPaymentMethodDto> result))
                {
                    _logger.LogInformation("Returning cached Payment Method");
                    return result;
                }

                // If not in cache, fetch the database
                var paymentMethod = await _unitOfWork.PaymentMethods.GetAllAsync();
                var paymentMethodDto = _mapper.Map<IEnumerable<DisplayPaymentMethodDto>>(paymentMethod);

                // cache the result
                _cache.Set(cacheKey, paymentMethodDto, TimeSpan.FromMinutes(5));

                return paymentMethodDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the result.");
                throw;
            }
        }

        public async Task<DisplayPaymentMethodDto> GetPaymentMethodByIdAsync(Guid id)
        {
            var cacheKey = $"paymentmethod_{id}";
            try
            {
                _logger.LogInformation($"Fetching paymentMethod with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayPaymentMethodDto cachedpaymentMethod))
                {
                    _logger.LogInformation("Returning cached paymentMethod");
                    return cachedpaymentMethod;
                }

                // If not in cache, fetch from database
                var paymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(id);
                if (paymentMethod == null)
                {
                    _logger.LogWarning($"Payment Method with ID: {id} was not found.");
                    return null;
                }

                var paymentMethodDto = _mapper.Map<DisplayPaymentMethodDto>(paymentMethod);

                // Cache the result
                _cache.Set(cacheKey, paymentMethodDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return paymentMethodDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the paymentMethod with ID {id}.");
                throw;
            }
        }

        public async Task<CreatePaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto paymentMethodDto)
        {
            if (paymentMethodDto == null)
            {
                throw new ArgumentNullException(nameof(paymentMethodDto), "The Manufacturer DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var paymentMethod = _mapper.Map<PaymentMethod>(paymentMethodDto);

                // Log information about paymentmethod creation
                _logger.LogInformation($"Creating a new payment method {paymentMethodDto.PaymentMethodName}");

                // Add the paymentMethod to the repository
                await _unitOfWork.PaymentMethods.CreateAsync(paymentMethod);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful paymentMethod creation
                _logger.LogInformation($"payement with ID: {paymentMethod.PaymentMethodId} created successfully.");

                // Invalidate all cache for GetAllPaymentMethodAsync
                _cache.Remove("all_paymentmethod");

                // Map Entity back to DTO from return
                return _mapper.Map<CreatePaymentMethodDto>(paymentMethod);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the paymentMethod.");
                throw new ApplicationException("An error occurred while adding the payment Method", ex);
            }
        }

        public async Task<CreatePaymentMethodDto> UpdatePaymentMethodAsync(Guid id, CreatePaymentMethodDto updatePaymentMethodDto)
        {
            if (updatePaymentMethodDto == null)
            {
                throw new ArgumentNullException(nameof(updatePaymentMethodDto), "The update paymentMethod DTO cannot be null.");
            }

            try
            {
                // Fetch the existing paymentMethod from the repository
                var existingPaymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(id);

                if (existingPaymentMethod == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing paymentmethod with {id}");
                    throw new KeyNotFoundException($"Payment Method with ID {id} not found.");
                }

                // Log Information about the paymentMethod before updating
                _logger.LogInformation($"Updating paymentMethod with ID: {id} - Current Name: {existingPaymentMethod.PaymentMethodName}");

                // Map the updates from DTO to the existing payment Method
                _mapper.Map(updatePaymentMethodDto, existingPaymentMethod);

                // Log the details of the update
                _logger.LogInformation($"Updatng manufacturer with ID: {id} - New Name: {updatePaymentMethodDto.PaymentMethodName}");

                // Update the payment Method using unitofwork with paymentMethodId
                await _unitOfWork.PaymentMethods.UpdateAsync(existingPaymentMethod, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful manufacturer update
                _logger.LogInformation($"Manufacturer with ID: {id} update successfully.");

                // Invalidate caches for this manufacturer and all manufacturers
                _cache.Remove($"paymentmethod_{id}");
                _cache.Remove("all_paymentmethod");

                // Map the updated entity back to DTO
                return _mapper.Map<CreatePaymentMethodDto>(existingPaymentMethod);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the payment method with ID: {id}");
                throw new ApplicationException("An error occurred while updating the paymentMethod", ex);
            }
        }

        public async Task<bool> DeletePaymentMethodAsync(Guid id)
        {
            try
            {
                // Fetch the existing paymentMethod from the unitofwork
                var existingPaymentMethod = await _unitOfWork.PaymentMethods.GetByIdAsync(id);

                if (existingPaymentMethod == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing paymentMethod with ID: {id}");
                    return false; // Return false if the paymentMethod was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting paymentMethod with ID: {id} - Name: {existingPaymentMethod.PaymentMethodName}");

                // Delete the payment Method from the repository
                await _unitOfWork.PaymentMethods.DeleteAsync(id);

                // Invalidate caches for this payment Method and all paymentMethods
                _cache.Remove($"paymentmethod_{id}");
                _cache.Remove("all_paymentmethod");

                // Log successful paymentmethod deletion
                _logger.LogInformation($"Payment Method with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the paymentMethod with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the paymentMethod", ex);
            }
        }
    }
}
