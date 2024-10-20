using IMS.Core.Entities;
using IMS.DataAccess.Repositories.BaseRepository.Interface;

namespace IMS.DataAccess.Repositories.CustomerRepository.Interface
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<int> GetAllCustomersCountAsync();
    }
}
