using Azure.Core;

namespace IMS.Application.Common
{
    public static class CacheKeys
    {
        // OrderStatus
        public static string OrderStatus => "all_orderstatus";
        public static string GetOrderStatusById(Guid orderStatusId) => $"orderstatus_{orderStatusId}";

        // OrderItem
        public static string OrderItems => "all_orderitem";
        public static string GetOrderItemById(Guid orderItemId) => $"orderitem_{orderItemId}";

        // Order
        public static string Order => "all_order";
        public static string GetOrderById(Guid orderId) => $"order_{orderId}";
    }
}
