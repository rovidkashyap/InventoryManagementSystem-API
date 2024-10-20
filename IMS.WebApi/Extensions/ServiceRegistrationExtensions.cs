using IMS.Application.Services.CategoryServices.Interface;
using IMS.Application.Services.CategoryServices.Service;
using IMS.Application.Services.CustomerServices.Interface;
using IMS.Application.Services.CustomerServices.Service;
using IMS.Application.Services.InventoryServices.Interface;
using IMS.Application.Services.InventoryServices.Service;
using IMS.Application.Services.ManufacturerServices.Interface;
using IMS.Application.Services.ManufacturerServices.Service;
using IMS.Application.Services.OrderItemServices.Interface;
using IMS.Application.Services.OrderItemServices.Service;
using IMS.Application.Services.OrderServices.Interface;
using IMS.Application.Services.OrderServices.Service;
using IMS.Application.Services.OrderStatusServices.Interface;
using IMS.Application.Services.OrderStatusServices.Service;
using IMS.Application.Services.PaymentMethodServices.Interface;
using IMS.Application.Services.PaymentMethodServices.Service;
using IMS.Application.Services.ProductServices.Interface;
using IMS.Application.Services.ProductServices.Service;
using IMS.Application.Services.SupplierServices.Interface;
using IMS.Application.Services.SupplierServices.Service;
using IMS.Application.Services.TransactionTypeServices.Interface;
using IMS.Application.Services.TransactionTypeServices.Service;
using IMS.Application.Services.UnitOfMeasureServices.Interface;
using IMS.Application.Services.UnitOfMeasureServices.Service;
using IMS.DataAccess.Repositories.BaseRepository.Interface;
using IMS.DataAccess.Repositories.BaseRepository.Repository;
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

namespace IMS.WebApi.Extensions
{
    public static class ServiceRegistrationExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services)
        {
            // Register Respositories
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IManufacturerRepository, ManufacturerRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ITransactionTypeRepository, TransactionTypeRepository>();
            services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();

            // Register Services
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IManufacturerService, ManufacturerService>();
            services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
            services.AddScoped<ITransactionTypeService, TransactionTypeService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderItemService, OrderItemService>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();

            return services;
        }
    }
}
