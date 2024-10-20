using IMS.Application.DTOs.Suppliers;

namespace IMS.Application.Services.SupplierServices.Interface
{
    public interface ISupplierService
    {
        Task<IEnumerable<DisplaySuppliersDto>> GetAllSuppliersAsync();
        Task<DisplaySuppliersDto> GetSupplierByIdAsync(Guid id);
        Task<CreateSupplierDto> CreateSupplierAsync(CreateSupplierDto supplierDto);
        Task<CreateSupplierDto> UpdateSupplierAsync(Guid id, CreateSupplierDto updateSupplierDto);
        Task<bool> DeleteSupplierAsync(Guid id);

        Task<int> GetAllSuppliersCountAsync();
    }
}
