using IMS.Application.DTOs.Customers;

namespace IMS.Application.Services.CustomerServices.Interface
{
    public interface ICustomerService
    {
        Task<IEnumerable<DisplayCustomerDto>> GetAllCustomerAsync();
        Task<DisplayCustomerDto> GetCustomerByIdAsync(Guid id);
        Task<CreateCustomerDto> CreateCustomerAsync(CreateCustomerDto customerDto);
        Task<CreateCustomerDto> UpdateCustomerAsync(Guid id, CreateCustomerDto customerDto);
        Task<bool> DeleteCustomer(Guid id);

        Task<int> GetAllCustomersCountAsync();
    }
}
