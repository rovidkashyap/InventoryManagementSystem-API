using Azure.Core;
using IMS.Core.Entities;
using System.Text;

namespace IMS.Application.Common.Utility
{
    public static class OrderCalculator
    {
        private const decimal GstRate = 0.18m; // 18% GST rate for simplicity
        private const decimal FixedDiscountAmount = 100; // Fixed Discount amount

        public static decimal CalculateTaxAmount(decimal totalPrice)
        {
            return totalPrice * GstRate;
        }

        public static decimal CalculateDiscountAmount()
        {
            return FixedDiscountAmount;
        }

        public static decimal CalculateShippingCost(decimal totalPrice)
        {
            if(totalPrice > 10000)
            {
                return 0;   // Free shipping for orders above 10,000 INR
            }
            else if(totalPrice > 5000)
            {
                return 100; // Flat shipping cost of 100 INR for orders between 5,000 and 10,000 INR
            }
            else
            {
                return 200; // Flat shipping cost of 200 INR for orders below 5,000 INR
            }
        }

        public static decimal CalculateTotalPrice(ICollection<OrderItem> orderItems)
        {
            return orderItems.Sum(item => item.Quantity * item.UnitPrice);
        }

        public static decimal CalculateNetAmount(decimal totalPrice, decimal taxAmount, decimal discountAmount, decimal shippingCost)
        {
            return totalPrice + taxAmount - discountAmount + shippingCost;
        }
    }
}
