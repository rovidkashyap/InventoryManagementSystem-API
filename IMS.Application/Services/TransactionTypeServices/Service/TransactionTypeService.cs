using AutoMapper;
using IMS.Application.DTOs.TransactionTypes;
using IMS.Application.Services.TransactionTypeServices.Interface;
using IMS.Core.Entities;
using IMS.DataAccess.UoW;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Services.TransactionTypeServices.Service
{
    public class TransactionTypeService : ITransactionTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private ILogger<TransactionTypeService> _logger;
        private readonly IMemoryCache _cache;
        public TransactionTypeService(IUnitOfWork unitOfWork, IMapper mapper,
            ILogger<TransactionTypeService> logger, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<DisplayTransactionTypeDto>> GetAllTransactionTypesAsync()
        {
            const string cacheKey = "all_transactiontype";
            try
            {
                // Check if data is in cache memory
                if (_cache.TryGetValue(cacheKey, out IEnumerable<DisplayTransactionTypeDto> result))
                {
                    _logger.LogInformation("Returning cached Transactions");
                    return result;
                }

                // If not in cache, fetch from database
                var transactions = await _unitOfWork.TransactionTypes.GetAllAsync();
                var transactionsDto = _mapper.Map<IEnumerable<DisplayTransactionTypeDto>>(transactions);

                // Cache the result
                _cache.Set(cacheKey, transactionsDto, TimeSpan.FromMinutes(5));

                return transactionsDto;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the result.");
                throw;
            }
        }

        public async Task<DisplayTransactionTypeDto> GetTransactionTypeByIdAsync(Guid id)
        {
            var cacheKey = $"transactiontype_{id}";
            try
            {
                _logger.LogInformation($"Fetching TransactionType with ID: {id}");

                // Check if the data is in cache
                if (_cache.TryGetValue(cacheKey, out DisplayTransactionTypeDto cachedTransaction))
                {
                    _logger.LogInformation("Returning cached transaction");
                    return cachedTransaction;
                }

                // If not in cache, fetch from database
                var transaction = await _unitOfWork.TransactionTypes.GetByIdAsync(id);
                if (transaction == null)
                {
                    _logger.LogWarning($"TransactionType with ID: {id} was not found.");
                    return null;
                }

                var transactionDto = _mapper.Map<DisplayTransactionTypeDto>(transaction);

                // Cache the result
                _cache.Set(cacheKey, transactionDto, TimeSpan.FromMinutes(5)); // Set appropriate cache duration

                return transactionDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching the transaction with ID {id}.");
                throw;
            }
        }

        public async Task<CreateTransactionTypeDto> CreateTransactionTypeAsync(CreateTransactionTypeDto transactionTypeDto)
        {
            if (transactionTypeDto == null)
            {
                throw new ArgumentNullException(nameof(transactionTypeDto), "The TransactionType DTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var transaction = _mapper.Map<TransactionType>(transactionTypeDto);

                // Log information about transactionType creation
                _logger.LogInformation($"Creating a new transactionType {transactionTypeDto.Name}");

                // Add the transactionType to the repository
                await _unitOfWork.TransactionTypes.CreateAsync(transaction);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful transaction creation
                _logger.LogInformation($"Transaction with ID: {transaction.TransactionTypeId} created successfully.");

                // Invalidate all cache for GetAllTransactionTypeAsync
                _cache.Remove("all_transactiontype");

                // Map Entity back to DTO from return
                return _mapper.Map<CreateTransactionTypeDto>(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the transactionType.");
                throw new ApplicationException("An error occurred while adding the transactionType", ex);
            }
        }

        public async Task<CreateTransactionTypeDto> UpdateTransactionTypeAsync(Guid id, CreateTransactionTypeDto updateTransactionTypeDto)
        {
            if (updateTransactionTypeDto == null)
            {
                throw new ArgumentNullException(nameof(updateTransactionTypeDto), "The update manufacturer DTO cannot be null.");
            }

            try
            {
                // Fetch the existing category from the repository
                var existingTransaction = await _unitOfWork.TransactionTypes.GetByIdAsync(id);

                if (existingTransaction == null)
                {
                    _logger.LogWarning($"Attempted to update a non-existing transaction with {id}");
                    throw new KeyNotFoundException($"TransactionType with ID {id} not found.");
                }

                // Log Information about the manufacturer before updating
                _logger.LogInformation($"Updating manufacturer with ID: {id} - Current Name: {existingTransaction.Name}");

                // Map the updates from DTO to the existing category
                _mapper.Map(updateTransactionTypeDto, existingTransaction);

                // Log the details of the update
                _logger.LogInformation($"Updatng manufacturer with ID: {id} - New Name: {updateTransactionTypeDto.Name}");

                // Update the transactionType using unitofwork with id
                await _unitOfWork.TransactionTypes.UpdateAsync(existingTransaction, id);

                // Save Changes to the database
                await _unitOfWork.CompleteAsync();

                // Log successful transactionType update
                _logger.LogInformation($"TransactionType with ID: {id} update successfully.");

                // Invalidate caches for this transactionType and all transactionTypes
                _cache.Remove($"transactiontype_{id}");
                _cache.Remove("all_transactiontype");

                // Map the updated entity back to DTO
                return _mapper.Map<CreateTransactionTypeDto>(existingTransaction);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while updating the transaction with ID: {id}");
                throw new ApplicationException("An error occurred while updating the transaction", ex);
            }
        }

        public async Task<bool> DeleteTransactionTypeDto(Guid id)
        {
            try
            {
                // Fetch the existing transaction from the unitofwork
                var existingTransaction = await _unitOfWork.TransactionTypes.GetByIdAsync(id);

                if (existingTransaction == null)
                {
                    _logger.LogWarning($"Attempting to delete a non-existing transaction with ID: {id}");
                    return false; // Return false if the transaction was not found
                }

                // Log the information before deletion
                _logger.LogInformation($"Deleting transaction with ID: {id} - Name: {existingTransaction.Name}");

                // Delete the manufacturer from the repository
                await _unitOfWork.TransactionTypes.DeleteAsync(id);

                // Invalidate caches for this transaction and all transactionTypes
                _cache.Remove($"transactiontype_{id}");
                _cache.Remove("all_transactiontype");

                // Log successful transactiontype deletion
                _logger.LogInformation($"TransactionType with ID: {id} deleted successfully.");

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, $"An error occurred while deleting the transactionType with ID: {id}");
                throw new ApplicationException("An error occurred while deleting the transactionType", ex);
            }
        }
    }
}
