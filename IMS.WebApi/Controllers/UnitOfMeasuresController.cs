using AutoMapper;
using IMS.Application.DTOs.UnitOfMeasures;
using IMS.Application.Services.UnitOfMeasureServices.Interface;
using IMS.DataAccess.UoW;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;

namespace IMS.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitOfMeasuresController : ControllerBase
    {
        private readonly IUnitOfMeasureService _unitService;
        private ILogger<UnitOfMeasuresController> _logger;
        public UnitOfMeasuresController(IUnitOfMeasureService unitService, ILogger<UnitOfMeasuresController> logger)
        {
            _unitService = unitService;
            _logger = logger;
        }

        // GET: api/unitofmeasure
        [HttpGet]
        public async Task<IActionResult> GetAllUnitOfMeasure()
        {
            _logger.LogInformation("Received request to get all units.");
            try
            {
                var result = await _unitService.GetAllUnitOfMeasureAsync();
                if(result == null || !result.Any())
                {
                   _logger.LogWarning("No Record Found.");
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while getting all unitofmeasure.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET: api/unitofmeasure/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUnitOfMeasureById(Guid id)
        {
            _logger.LogInformation($"Received request to get units with Id: {id}");

            try
            {
                var units = await _unitService.GetUnitOfMeasureByIdAsync(id);
                if (units == null)
                {
                    _logger.LogWarning($"Units with ID: {id} not found.");
                    return NotFound();
                }
                return Ok(units);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting the Units with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        //POST: api/unitofmeasure
        [HttpPost]
        public async Task<IActionResult> CreateUnits([FromBody] CreateUnitOfMeasureDto unitsOfMeasureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Received Request to create a new unitOfMeasure");

            try
            {
                var createdUnits = await _unitService.CreateUnitOfMeasureAsync(unitsOfMeasureDto);

                _logger.LogInformation($"UnitofMeasures with ID {unitsOfMeasureDto.UnitOfMeasureId} created.");
                return CreatedAtAction(nameof(GetUnitOfMeasureById), new { id = unitsOfMeasureDto.UnitOfMeasureId }, createdUnits);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the unitOfMeasure.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT: api/unitofmeasure/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUnitOfMeasure(Guid id, [FromBody] CreateUnitOfMeasureDto updateUnitDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  //400 Bad Request for Invalid input
            }

            _logger.LogInformation($"Received request to update unitofmeasure with ID: {id}");

            try
            {
                var updatedUnitOfMeasure = await _unitService.UpdateUnitOfMeasureAsync(id, updateUnitDto);
                if (updatedUnitOfMeasure == null)
                {
                    return NotFound();  // 400 Not Found is the UnitOfMeasure doesn't exists.
                }

                _logger.LogInformation($"UnitOfMeasure with ID: {id} update successfully.");
                return Ok(updatedUnitOfMeasure); // 200 OK with updated unitOfMeasure data
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the unitOfMeasure with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // DLETE: api/unitofmeasure/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUnit(Guid id)
        {
            _logger.LogInformation($"Received request to delete unitofmeasure with ID: {id}");

            try
            {
                var deleted = await _unitService.DeleteUnitOfMeasureAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning($"UnitOfMeasure with ID: {id} was not found or already deleted.");
                    return NotFound(); // 404 Not Found if the unitofmeasure doesn't exist
                }

                _logger.LogInformation($"UnitOfMeasure with ID: {id} deleted successfully.");
                return NoContent(); // 200 No Content on successful deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the unitofmeasure with ID: {id}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
