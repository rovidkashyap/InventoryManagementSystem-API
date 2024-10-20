namespace IMS.Application.DTOs.PaymentMethods
{
    public class DisplayPaymentMethodDto
    {
        public Guid PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
