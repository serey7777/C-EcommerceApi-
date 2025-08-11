using Microsoft.AspNetCore.Mvc;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

namespace WebApplicationProductAPI.Repositories.SupplierRepo
{
    public interface ISupplierRepository
    {
        //return Task list of Supplier Domain
        Task<List<SupplierDomain>>GetAllAsync();
        Task<SupplierDomain?> GetSupplierAsync(int id);

        Task<SupplierDomain> AddPostAsync(SupplierDomain supplier);

     
        Task<SupplierDomain?> DeleteSupplierAsync(int id);
        Task<SupplierDomain> UpdateSupplierAsync(int id, SupplierUpdateDto dto);
    }
}
