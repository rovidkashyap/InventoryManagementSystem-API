using IMS.Application.DTOs.PaymentMethods;

namespace IMS.Application.Services.PaymentMethodServices.Interface
{
    public interface IPaymentMethodService
    {
        Task<IEnumerable<DisplayPaymentMethodDto>> GetPaymentMethodAsync();
        Task<DisplayPaymentMethodDto> GetPaymentMethodByIdAsync(Guid id);
        Task<CreatePaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto paymentMethodDto);
        Task<CreatePaymentMethodDto> UpdatePaymentMethodAsync(Guid id, CreatePaymentMethodDto updatePaymentMethodDto);
        Task<bool> DeletePaymentMethodAsync(Guid id);
    }
}
