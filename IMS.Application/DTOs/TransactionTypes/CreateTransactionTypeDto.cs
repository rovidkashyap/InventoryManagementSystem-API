namespace IMS.Application.DTOs.TransactionTypes
{
    public class CreateTransactionTypeDto
    {
        public Guid TransactionTypeId { get; set; }
        public string Name { get; set; }    // (e.g., Purchase, Sale, Return, Adjustment)
        public string? Description { get; set; }
    }
}
