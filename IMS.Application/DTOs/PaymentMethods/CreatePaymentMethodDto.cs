namespace IMS.Application.DTOs.PaymentMethods
{
    public class CreatePaymentMethodDto
    {
        public Guid PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
