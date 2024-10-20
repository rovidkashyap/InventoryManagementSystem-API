using IMS.Application.DTOs.UnitOfMeasures;

namespace IMS.Application.Services.UnitOfMeasureServices.Interface
{
    public interface IUnitOfMeasureService
    {
        Task<IEnumerable<DisplayUnitOfMeasureDto>> GetAllUnitOfMeasureAsync();
        Task<DisplayUnitOfMeasureDto> GetUnitOfMeasureByIdAsync(Guid unitOfMeasureId);
        Task<CreateUnitOfMeasureDto> CreateUnitOfMeasureAsync(CreateUnitOfMeasureDto unitOfMeasureDto);
        Task<CreateUnitOfMeasureDto> UpdateUnitOfMeasureAsync(Guid Id, CreateUnitOfMeasureDto unitOfMeasureDto);
        Task<bool> DeleteUnitOfMeasureAsync(Guid Id);
    }
}
