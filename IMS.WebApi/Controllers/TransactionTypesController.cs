using IMS.Application.DTOs.Manufacturers;
using IMS.Application.DTOs.TransactionTypes;
using IMS.Application.Services.ManufacturerServices.Interface;
using IMS.Application.Services.TransactionTypeServices.Interface;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionTypesController : ControllerBase
    {
        private readonly ITransactionTypeService _transactionService;
        private ILogger<TransactionTypesController> _logger;
        public TransactionTypesController(ITransactionTypeService transactionService, ILogger<TransactionTypesController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        // GET: api/transactiontypes
        [HttpGet]
        public async Task<IActionResult> GetAllTransactionTypes()
        {
            _logger.LogInformation("Received request to get all transactiontypes.");
            try
            {
                var transaction = await _transactionService.GetAllTransactionTypesAsync();

                if (transaction == null || !transaction.Any())
                {
                    _logger.LogInformation("No Record Found");
                    return NoContent();
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all transactions.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET:api/transaction/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetTransactionTypeById(Guid id)
        {
            _logger.LogInformation($"Received request to get transaction with Id: {id}");

            try
            {
                var transaction = await _transactionService.GetTransactionTypeByIdAsync(id);
                if (transaction == null)
                {
                    _logger.LogWarning($"Transaction with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the Transaction with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //POST: api/transaction
        [HttpPost]
        public async Task<IActionResult> CreateTransactionType([FromBody] CreateTransactionTypeDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request for Invalid inputs
            }

            _logger.LogInformation($"Receieved request to create a new transaction");

            try
            {
                var createdTransaction = await _transactionService.CreateTransactionTypeAsync(transactionDto);

                _logger.LogInformation($"Transaction with ID: {transactionDto.TransactionTypeId} created.");
                return CreatedAtAction(nameof(GetTransactionTypeById), new { id = createdTransaction.TransactionTypeId }, createdTransaction);
                // 201 created with location header
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occrred while creating the transaction.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //PUT: api/transaction
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateTransactionType(Guid id, CreateTransactionTypeDto transactionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update transaction with ID: {id}");

            try
            {
                var updatedTransaction = await _transactionService.UpdateTransactionTypeAsync(id, transactionDto);
                if (transactionDto == null)
                {
                    return NotFound();  // 400 Not Found is the transaction doesn't exists.
                }

                _logger.LogInformation($"Transaction with ID: {id} update successfully.");
                return Ok(updatedTransaction); // 200 OK with updated transaction data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the transaction with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DELETE: api/transaction
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            _logger.LogInformation($"Received request to delete transaction with ID: {id}");

            try
            {
                var deleted = await _transactionService.DeleteTransactionTypeDto(id);
                if (!deleted)
                {
                    _logger.LogWarning($"Transaction with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the transaction doesn't exist
                }

                _logger.LogInformation($"Transaction with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the transaction with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
