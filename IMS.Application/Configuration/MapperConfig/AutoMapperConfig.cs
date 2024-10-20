using AutoMapper;
using IMS.Application.DTOs.Categories;
using IMS.Application.DTOs.Customers;
using IMS.Application.DTOs.Inventories;
using IMS.Application.DTOs.Manufacturers;
using IMS.Application.DTOs.OrderItems;
using IMS.Application.DTOs.Orders;
using IMS.Application.DTOs.OrderStatus;
using IMS.Application.DTOs.PaymentMethods;
using IMS.Application.DTOs.Products;
using IMS.Application.DTOs.Suppliers;
using IMS.Application.DTOs.TransactionTypes;
using IMS.Application.DTOs.UnitOfMeasures;
using IMS.Core.Entities;
using System.ComponentModel.Design;

namespace IMS.Application.Configuration.MapperConfig
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Category Dto Mapping
            CreateMap<Category, DisplayCategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();

            //Manufacturer Dto Mapping
            CreateMap<Manufacturer, DisplayManufacturerDto>().ReverseMap();
            CreateMap<Manufacturer, CreateManufacturerDto>().ReverseMap();

            // UnitOfMeasure Dto Mapping
            CreateMap<UnitOfMeasure, DisplayUnitOfMeasureDto>().ReverseMap();
            CreateMap<UnitOfMeasure, CreateUnitOfMeasureDto>().ReverseMap();

            // TransactionType Dto Mapping
            CreateMap<TransactionType, DisplayTransactionTypeDto>().ReverseMap();
            CreateMap<TransactionType, CreateTransactionTypeDto>().ReverseMap();

            // PaymentMethod Dto Mapping
            CreateMap<PaymentMethod, DisplayPaymentMethodDto>().ReverseMap();
            CreateMap<PaymentMethod, CreatePaymentMethodDto>().ReverseMap();

            // Supplier Dto Mapping
            CreateMap<Supplier, DisplaySuppliersDto>().ReverseMap();
            CreateMap<Supplier, CreateSupplierDto>().ReverseMap();

            // Product Dto Mapping
            CreateMap<Product, DisplayProductDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ManufacturerName, opt => opt.MapFrom(src => src.Manufacturer.ManufacturerName))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.UnitOfMeasure.UnitName))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions))
                .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => src.ReorderLevel))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ReverseMap();

            CreateMap<Product, CreateProductDto>().ReverseMap();

            CreateMap<Product, ProductsByManufacturerDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.ProductCode, opt => opt.MapFrom(src => src.ProductCode))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.UnitOfMeasureName, opt => opt.MapFrom(src => src.UnitOfMeasure.UnitName))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
                .ForMember(dest => dest.Dimensions, opt => opt.MapFrom(src => src.Dimensions))
                .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => src.ReorderLevel))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

            // Customer Dto Mapping
            CreateMap<Customer, DisplayCustomerDto>().ReverseMap();
            CreateMap<Customer, CreateCustomerDto>().ReverseMap();

            // Inventory Dto Mapping
            CreateMap<Inventory, DisplayInventoryDto>()
                .ForMember(dest => dest.InventoryId, opt => opt.MapFrom(src => src.InventoryId))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.SupplierName))
                .ForMember(dest => dest.QuantityAdded, opt => opt.MapFrom(src => src.QuantityAdded))
                .ForMember(dest => dest.QuantityRemoved, opt => opt.MapFrom(src => src.QuantityRemoved))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.TransactionTypeName, opt => opt.MapFrom(src => src.TransactionType.Name))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy));

            CreateMap<Inventory, CreateInventoryDto>().ReverseMap();

            CreateMap<Inventory, DisplayRecentInventoryDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.TransactionName, opt => opt.MapFrom(src => src.TransactionType.Name))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.SupplierName))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
                .ForMember(dest => dest.QuantityAdded, opt => opt.MapFrom(src => src.QuantityAdded))
                .ForMember(dest => dest.QuantityRemoved, opt => opt.MapFrom(src => src.QuantityRemoved))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));

            // Order Dto Mapping
            CreateMap<Order, DisplayOrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod.PaymentMethodName))
                .ForMember(dest => dest.OrderStatusName, opt => opt.MapFrom(src => src.OrderStatus.OrderStatusName));

            CreateMap<Order, DisplayOrdersForOrderItemsDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.MapFrom(src => src.PaymentMethod.PaymentMethodName))
                .ForMember(dest => dest.OrderStatusName, opt => opt.MapFrom(src => src.OrderStatus.OrderStatusName));

            CreateMap<Order, CreateOrderDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
                .ForMember(dest => dest.ShippingCost, opt => opt.Ignore())
                .ForMember(dest => dest.NetAmount, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.CustomerName))
                .ForMember(dest => dest.OrderStatusName, opt => opt.MapFrom(src => src.OrderStatus.OrderStatusName))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate));

            // OrderItem Dto Mapping
            CreateMap<OrderItem, CreateOrderItemDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
                .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TaxAmount, opt => opt.Ignore())
                .ForMember(dest => dest.NetPrice, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<OrderItem, DisplayOrderItemDto>().ReverseMap();

            CreateMap<OrderItem, DisplayOrderItemsForOrdersDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName));

            // OrderStatus Dto Mapping
            CreateMap<OrderStatus, DisplayOrderStatusDto>().ReverseMap();
            CreateMap<OrderStatus, CreateOrderStatusDto>().ReverseMap();
        }
    }
}
