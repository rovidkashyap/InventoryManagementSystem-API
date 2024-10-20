using IMS.Application.DTOs.Manufacturers;

namespace IMS.Application.Services.ManufacturerServices.Interface
{
    public interface IManufacturerService
    {
        Task<IEnumerable<DisplayManufacturerDto>> GetAllManufacturersAsync();
        Task<DisplayManufacturerDto> GetManufacturerByIdAsync(Guid manufacturerId);
        Task<CreateManufacturerDto> CreateManufacturerAsync(CreateManufacturerDto manufacturerDto);
        Task<CreateManufacturerDto> UpdateManufacturerAsync(Guid Id, CreateManufacturerDto manufacturerDto);
        Task<bool> DeleteManufacturerAsync(Guid Id);
    }
}
