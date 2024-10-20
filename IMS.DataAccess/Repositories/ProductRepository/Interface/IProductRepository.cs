using IMS.Core.Entities;
using IMS.DataAccess.Repositories.BaseRepository.Interface;

namespace IMS.DataAccess.Repositories.ProductRepository.Interface
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsIncludeDetails();
        Task<Product> GetProductByIdIncludeDetails(Guid id);
        Task<IEnumerable<Product>> GetProductsByManufacturerId(Guid manufacturerId);

        Task<int> GetAllProductsCountAsync();
    }
}
