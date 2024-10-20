namespace IMS.Application.DTOs.Inventories
{
    public class DisplayRecentInventoryDto
    {
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public int QuantityAdded { get; set; }
        public int QuantityRemoved { get; set; }
        public int QuantityInStock { get; set; }
        public string TransactionName { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
