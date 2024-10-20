using IMS.Core.Entities;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
using IMS.DataAccess.Repositories.CustomerRepository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.Repositories.CustomerRepository.Repository
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly IMSContext _context;
        public CustomerRepository(IMSContext context, ILogger<CustomerRepository> logger) 
            : base(context, logger)
        {
            _context = context;
        }

        public async Task<int> GetAllCustomersCountAsync()
        {
            try
            {
                return await _context.Customers.CountAsync();
            }
            catch(Exception ex)
            {
                throw new ArgumentNullException("An error occurred while counting customers.", ex);
            }
        }
    }
}
