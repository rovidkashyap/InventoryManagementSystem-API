using AutoMapper;
using IMS.Application.DTOs.Customers;
using IMS.Application.DTOs.Products;
using IMS.Application.Services.CustomerServices.Interface;
using IMS.Application.Services.ProductServices.Service;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.CustomerServices.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<CustomerService> _logger;
        private IMemoryCache _cache;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerService> logger,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<int> GetAllCustomersCountAsync()
        {
            try
            {
                return await _unitOfWork.Customers.GetAllCustomersCountAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get customer count.");
                throw new ArgumentNullException("Failed to get customers count.", ex);
            }
        }

        public async Task<IEnumerable<DisplayCustomerDto>> GetAllCustomerAsync()
        {
            const string cacheKey = "all_customer";
            try
            {
                _logger.LogInformation("Fetching all customers");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayCustomerDto> result))
                {
                    _logger.LogInformation("Returning cached customer.");
                    return result;
                }

                // If Not in cache, fetch from database
                var customers = await _unitOfWork.Customers.GetAllAsync();
                var customerDto = _mapper.Map<IEnumerable<DisplayCustomerDto>>(customers);

                // Cache the result
                _cache.Set(cacheKey, customerDto, TimeSpan.FromMinutes(5));  // Set appropriate cache duration

                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all customers");
                throw;
            }
        }

        public async Task<DisplayCustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var cacheKey = $"customer_{id}";
            try
            {
                _logger.LogInformation($"Fetching customer with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayCustomerDto cachedCustomer))
                {
                    _logger.LogInformation("Returning cached customer");
                    return cachedCustomer;
                }

                // If not in cache, fetch from database
                var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                if (customer == null)
                {
                    _logger.LogWarning($"Customer with ID: {id} was not found.");
                    return null;
                }

                var customerDto = _mapper.Map<DisplayCustomerDto>(customer);

                // Cache the result
                _cache.Set(cacheKey, customerDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return customerDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the product with ID {id}.");
                throw;
            }
        }

        public async Task<CreateCustomerDto> CreateCustomerAsync(CreateCustomerDto customerDto)
        {
            if (customerDto == null)
            {
                throw new ArgumentNullException(nameof(customerDto), "The customer DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var customer = _mapper.Map<Customer>(customerDto);

                // Log Information about customer creation
                _logger.LogInformation($"Creating a new customer: {customerDto.CustomerName}");

                // Add the customer to the repository
                await _unitOfWork.Customers.CreateAsync(customer);

                // Save changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successfull customer creation
                _logger.LogInformation($"Customer with ID: {customer.CustomerId} created successfully.");

                // Invalidate all cache for GetAllCustomerAsync
                _cache.Remove("all_customer");

                // Map Entity back to DTO for return
                return _mapper.Map<CreateCustomerDto>(customer);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the customer.");
                throw new ApplicationException("An error occured while adding the customer", ex);
            }
        }

        public async Task<CreateCustomerDto> UpdateCustomerAsync(Guid id, CreateCustomerDto customerDto)
        {
            if (customerDto == null)
            {
                throw new ArgumentNullException(nameof(customerDto), "The product Dto cannot be null.");
            }

            try
            {
                // Fetch the existing Product from the repository
                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(id);

                if (existingCustomer == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing customer with {id}");
                    throw new KeyNotFoundException($"Customer with ID {id} not found.");
                }

                // Log Information about the customer before updating
                _logger.LogInformation($"Updating customer with ID: {id} - Current Name: {existingCustomer.CustomerName}");

                // Map the updates from DTO to the existing customer
                _mapper.Map(customerDto, existingCustomer);

                // Log the details of the update
                _logger.LogInformation($"Updatng customer with ID: {id} - New Name: {customerDto.CustomerName}");

                // Update the customer using unitofwork with customerId
                await _unitOfWork.Customers.UpdateAsync(existingCustomer, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful customer update
                _logger.LogInformation($"Customer with ID: {id} update successfully.");

                // Invalidate caches for this customer and all customers
                _cache.Remove($"customer_{id}");
                _cache.Remove("all_customer");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateCustomerDto>(existingCustomer);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the customer with ID: {id}");
                throw new ApplicationException("An error occurred while updating the customer", ex);
            }
        }

        public async Task<bool> DeleteCustomer(Guid id)
        {
            try
            {
                // Fetch the existing customer from the unitofwork
                var existingCustomer = await _unitOfWork.Customers.GetByIdAsync(id);

                if (existingCustomer == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing customer with ID: {id}");
                    return false; // Return false if the customer was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting customer with ID: {id} - Name: {existingCustomer.CustomerName}");

                // Delete the customer from the repository
                await _unitOfWork.Customers.DeleteAsync(id);

                // Invalidate caches for this customer and all customers
                _cache.Remove($"customer_{id}");
                _cache.Remove("all_customer");

                // Log successful customer deletion
                _logger.LogInformation($"Customer with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the customer with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the customer", ex);
            }
        }
    }
}
