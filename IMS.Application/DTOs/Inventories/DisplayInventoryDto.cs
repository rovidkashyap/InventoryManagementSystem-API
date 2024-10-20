using Azure.Core.Pipeline;
using IMS.Core.Entities;

namespace IMS.Application.DTOs.Inventories
{
    public class DisplayInventoryDto
    {
        public Guid InventoryId { get; set; }
        public string ProductName { get; set; }
        public string SupplierName { get; set; }
        public int QuantityAdded { get; set; }
        public int QuantityRemoved { get; set; }
        public int QuantityInStock { get; set; }
        public decimal UnitPrice { get; set; }
        public string TransactionTypeName { get; set; }
        public string? Remark { get; set; }
        public DateTime TransactionDate { get; set; }

        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

    }
}

