namespace IMS.Application.Common.Utility
{
    public static class OrderItemCalculator
    {
        private const decimal GstRate = 0.18m;  // Example GST Rate of 18%

        public static decimal CalculateTotalPrice(int quantity, decimal unitPrice)
        {
            return quantity * unitPrice;
        }

        public static decimal CalculateTaxAmount(decimal totalPrice)
        {
            return totalPrice * GstRate;
        }

        public static decimal CalculateDiscountAmount(decimal totalPrice, decimal DiscountPercentage)
        {
            return totalPrice * (DiscountPercentage / 100);
        }

        public static decimal CalculateNetPrice(decimal totalPrice, decimal discountAmount, decimal taxAmount)
        {
            return totalPrice - discountAmount + taxAmount;
        }
    }
}
