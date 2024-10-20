namespace IMS.Application.DTOs.TransactionTypes
{
    public class DisplayTransactionTypeDto
    {
        public Guid TransactionTypeId { get; set; }
        public string Name { get; set; } 
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
