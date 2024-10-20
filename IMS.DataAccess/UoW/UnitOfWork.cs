using IMS.DataAccess.DataContext;
using IMS.DataAccess.Repositories.CategoryRepository.Interface;
using IMS.DataAccess.Repositories.CategoryRepository.Repository;
using IMS.DataAccess.Repositories.CustomerRepository.Interface;
using IMS.DataAccess.Repositories.CustomerRepository.Repository;
using IMS.DataAccess.Repositories.InventoryRepository.Interface;
using IMS.DataAccess.Repositories.InventoryRepository.Repository;
using IMS.DataAccess.Repositories.ManufacturerRepository.Interface;
using IMS.DataAccess.Repositories.ManufacturerRepository.Repository;
using IMS.DataAccess.Repositories.OrderItemRepository.Interface;
using IMS.DataAccess.Repositories.OrderItemRepository.Repository;
using IMS.DataAccess.Repositories.OrderRepository.Interface;
using IMS.DataAccess.Repositories.OrderRepository.Repository;
using IMS.DataAccess.Repositories.OrderStatusRepository.Interface;
using IMS.DataAccess.Repositories.OrderStatusRepository.Repository;
using IMS.DataAccess.Repositories.PaymentMethodRepository.Interface;
using IMS.DataAccess.Repositories.PaymentMethodRepository.Repository;
using IMS.DataAccess.Repositories.ProductRepository.Interface;
using IMS.DataAccess.Repositories.ProductRepository.Repository;
using IMS.DataAccess.Repositories.SupplierRepository.Interface;
using IMS.DataAccess.Repositories.SupplierRepository.Repository;
using IMS.DataAccess.Repositories.TransactionTypeRepository.Interface;
using IMS.DataAccess.Repositories.TransactionTypeRepository.Repository;
using IMS.DataAccess.Repositories.UnitOfMeasureRepository.Interface;
using IMS.DataAccess.Repositories.UnitOfMeasureRepository.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace IMS.DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMSContext _context;
        private readonly ILoggerFactory _loggerFactory;
        private IDbContextTransaction _transaction;

        private IProductRepository _product;
        private ICustomerRepository _customer;
        private ICategoryRepository _category;
        private IInventoryRepository _inventory;
        private IManufacturerRepository _manufacturer;
        private IOrderItemRepository _orderItem;
        private IOrderRepository _order;
        private IPaymentMethodRepository _paymentMethod;
        private ISupplierRepository _supplier;
        private ITransactionTypeRepository _transactionType;
        private IUnitOfMeasureRepository _unitOfMeasure;
        private IOrderStatusRepository _orderStatus;

        public UnitOfWork(IMSContext context, ILoggerFactory loggerFactory, IProductRepository product, 
            ICustomerRepository customer, ICategoryRepository category, IInventoryRepository inventory, 
            IManufacturerRepository manufacturer, IOrderItemRepository orderItem, IOrderRepository order, 
            IPaymentMethodRepository paymentMethod, ISupplierRepository supplier,
            ITransactionTypeRepository transactionType, IUnitOfMeasureRepository unitOfMeasure,
            IOrderStatusRepository orderStatus)
        {
            _context = context;
            _loggerFactory = loggerFactory;
            _product = product;
            _customer = customer;
            _category = category;
            _inventory = inventory;
            _manufacturer = manufacturer;
            _orderItem = orderItem;
            _order = order;
            _paymentMethod = paymentMethod;
            _supplier = supplier;
            _transactionType = transactionType;
            _unitOfMeasure = unitOfMeasure;
            _orderStatus = orderStatus;
        }

        public ICustomerRepository Customers
        {
            get
            {
                if(_customer == null)
                {
                    var logger = _loggerFactory.CreateLogger<CustomerRepository>();
                    _customer = new CustomerRepository(_context, logger);
                }
                return _customer;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                if(_category == null)
                {
                    var logger = _loggerFactory.CreateLogger<CategoryRepository>();
                    _category = new CategoryRepository(_context, logger);
                }
                return _category;
            }
        }

        public IInventoryRepository Inventories
        {
            get
            {
                if (_inventory == null)
                {
                    var logger = _loggerFactory.CreateLogger<InventoryRepository>();
                    _inventory = new InventoryRepository(_context, logger);
                }
                return _inventory;
            }
        }

        public IManufacturerRepository Manufacturers
        {
            get
            {
                if (_manufacturer == null)
                {
                    var logger = _loggerFactory.CreateLogger<ManufacturerRepository>();
                    _manufacturer = new ManufacturerRepository(_context, logger);
                }
                return _manufacturer;
            }
        }

        public IOrderItemRepository OrderItems
        {
            get
            {
                if (_orderItem == null)
                {
                    var logger = _loggerFactory.CreateLogger<OrderItemRepository>();
                    _orderItem = new OrderItemRepository(_context, logger);
                }
                return _orderItem;
            }
        }

        public IOrderRepository Orders
        {
            get
            {
                if (_order == null)
                {
                    var logger = _loggerFactory.CreateLogger<OrderRepository>();
                    _order = new OrderRepository(_context, logger);
                }
                return _order;
            }
        }

        public IPaymentMethodRepository PaymentMethods
        {
            get
            {
                if (_paymentMethod == null)
                {
                    var logger = _loggerFactory.CreateLogger<PaymentMethodRepository>();
                    _paymentMethod = new PaymentMethodRepository(_context, logger);
                }
                return _paymentMethod;
            }
        }

        public IProductRepository Products
        {
            get
            {
                if (_product == null)
                {
                    var logger = _loggerFactory.CreateLogger<ProductRepository>();
                    _product = new ProductRepository(_context, logger);
                }
                return _product;
            }
        }

        public ISupplierRepository Suppliers
        {
            get
            {
                if (_supplier == null)
                {
                    var logger = _loggerFactory.CreateLogger<SupplierRepository>();
                    _supplier = new SupplierRepository(_context, logger);
                }
                return _supplier;
            }
        }

        public ITransactionTypeRepository TransactionTypes
        {
            get
            {
                if (_transactionType == null)
                {
                    var logger = _loggerFactory.CreateLogger<TransactionTypeRepository>();
                    _transactionType = new TransactionTypeRepository(_context, logger);
                }
                return _transactionType;
            }
        }

        public IUnitOfMeasureRepository UnitOfMeasures
        {
            get
            {
                if (_unitOfMeasure == null)
                {
                    var logger = _loggerFactory.CreateLogger<UnitOfMeasureRepository>();
                    _unitOfMeasure = new UnitOfMeasureRepository(_context, logger);
                }
                return _unitOfMeasure;
            }
        }

        public IOrderStatusRepository OrderStatus
        {
            get
            {
                if(_orderStatus == null)
                {
                    var logger = _loggerFactory.CreateLogger<OrderStatusRepository>();
                    _orderStatus = new OrderStatusRepository(_context, logger);
                }
                return _orderStatus;
            }
        }

        public async Task<IDbContextTransaction> BeginTransctionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
            return _transaction;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
