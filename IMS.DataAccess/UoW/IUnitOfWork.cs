using IMS.DataAccess.Repositories.CategoryRepository.Interface;
using IMS.DataAccess.Repositories.CustomerRepository.Interface;
using IMS.DataAccess.Repositories.InventoryRepository.Interface;
using IMS.DataAccess.Repositories.ManufacturerRepository.Interface;
using IMS.DataAccess.Repositories.OrderItemRepository.Interface;
using IMS.DataAccess.Repositories.OrderRepository.Interface;
using IMS.DataAccess.Repositories.OrderStatusRepository.Interface;
using IMS.DataAccess.Repositories.PaymentMethodRepository.Interface;
using IMS.DataAccess.Repositories.ProductRepository.Interface;
using IMS.DataAccess.Repositories.SupplierRepository.Interface;
using IMS.DataAccess.Repositories.TransactionTypeRepository.Interface;
using IMS.DataAccess.Repositories.UnitOfMeasureRepository.Interface;
using Microsoft.EntityFrameworkCore.Storage;

namespace IMS.DataAccess.UoW
{
    public interface IUnitOfWork
    {
        ICustomerRepository Customers { get; }
        ICategoryRepository Categories { get; }
        IInventoryRepository Inventories { get; }
        IManufacturerRepository Manufacturers { get; }
        IOrderItemRepository OrderItems { get; }
        IOrderRepository Orders { get; }
        IPaymentMethodRepository PaymentMethods { get; }
        IProductRepository Products { get; }
        ISupplierRepository Suppliers { get; }
        ITransactionTypeRepository TransactionTypes { get; }
        IUnitOfMeasureRepository UnitOfMeasures { get; }
        IOrderStatusRepository OrderStatus { get; }

        Task<int> CompleteAsync();  // Save Changes to the database
        Task<IDbContextTransaction> BeginTransctionAsync();
    }
}
