using IMS.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace IMS.DataAccess.Seed
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            SeedPaymentMethods(modelBuilder);
            SeedTransactionTypes(modelBuilder);
            SeedUnitOfMeasures(modelBuilder);
            SeedManufacturers(modelBuilder);
            SeedOrderStatus(modelBuilder);
        }

        private static void SeedOrderStatus(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Pending",
                    Description = "Your Order has been Pending."
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Shipped",
                    Description = "Your order has been Shipped."
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Completed",
                    Description = "Your Order has been Completed."
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Delivered",
                    Description = "Your Order has been Delivered."
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Returned",
                    Description = "Your Order has been Returned."
                },
                new OrderStatus
                {
                    OrderStatusId = Guid.NewGuid(),
                    OrderStatusName = "Failed",
                    Description = "Your Order has been Failed."
                });
        }

        private static void SeedPaymentMethods(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PaymentMethod>().HasData(
                new PaymentMethod
                {
                    PaymentMethodId = Guid.NewGuid(),
                    PaymentMethodName = "Credit Card",
                    Description = "Pay using Credit Card",
                    IsActive = true
                },
                new PaymentMethod
                {
                    PaymentMethodId = Guid.NewGuid(),
                    PaymentMethodName = "Debit Card",
                    Description = "Pay using Debit Card",
                    IsActive = true
                },
                new PaymentMethod
                {
                    PaymentMethodId = Guid.NewGuid(),
                    PaymentMethodName = "NetBanking",
                    Description = "Pay using NetBanking",
                    IsActive = true
                },
                new PaymentMethod
                {
                    PaymentMethodId = Guid.NewGuid(),
                    PaymentMethodName = "Cash",
                    Description = "Pay using Cash",
                    IsActive = true
                });
        }

        private static void SeedTransactionTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionType>().HasData(
                new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Purchase",
                    Description = "Items purchased"
                },
                new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Sale",
                    Description = "Item Sold"
                },
                new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Return",
                    Description = "Item Return"
                },
                new TransactionType
                {
                    TransactionTypeId = Guid.NewGuid(),
                    Name = "Adjustment",
                    Description = "Item Adjustment"
                });
        }

        private static void SeedUnitOfMeasures(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnitOfMeasure>().HasData(
                new UnitOfMeasure
                { UnitOfMeasureId = Guid.NewGuid(), UnitName = "Pieces", Abbreviation = "pcs" },
                new UnitOfMeasure
                { UnitOfMeasureId = Guid.NewGuid(), UnitName = "Kilogram", Abbreviation = "kg" },
                new UnitOfMeasure
                { UnitOfMeasureId = Guid.NewGuid(), UnitName = "Gram", Abbreviation = "gm" });
        }

        private static void SeedManufacturers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer
                {
                    ManufacturerId = Guid.NewGuid(),
                    ManufacturerName = "MItechRo Solutions",
                    ContactInfo = "Rovid Kashyap",
                    Address = "Nagla Parma, Jeoni Mandi, Agra"
                },
                new Manufacturer
                {
                    ManufacturerId = Guid.NewGuid(),
                    ManufacturerName = "Soulmate System Pvt. Ltd.",
                    ContactInfo = "Ashish Mittal",
                    Address = "Surya Apartment, New Agra Colony, Agra"
                });
        }
    }
}
