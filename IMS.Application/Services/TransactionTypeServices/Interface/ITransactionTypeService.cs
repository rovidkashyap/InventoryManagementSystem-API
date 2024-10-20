using IMS.Application.DTOs.TransactionTypes;

namespace IMS.Application.Services.TransactionTypeServices.Interface
{
    public interface ITransactionTypeService
    {
        Task<IEnumerable<DisplayTransactionTypeDto>> GetAllTransactionTypesAsync();
        Task<DisplayTransactionTypeDto> GetTransactionTypeByIdAsync(Guid id);
        Task<CreateTransactionTypeDto> CreateTransactionTypeAsync(CreateTransactionTypeDto transactionTypeDto);
        Task<CreateTransactionTypeDto> UpdateTransactionTypeAsync(Guid id, CreateTransactionTypeDto updateTransctionTypeDto);
        Task<bool> DeleteTransactionTypeDto(Guid id);
    }
}
