using Microsoft.AspNetCore.Mvc;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.ProductDTO;

namespace WebApplicationProductAPI.Repositories.ProductRepo
{
    public interface IProductRepository
    {
        //Parameter Domain Model
        Task<ProductDomain> AddProductAsync(ProductDomain productDomain);
        Task<List<ProductDomain?>> GetAllProductAsync(string? filterOn = null, string? filterQuery = null);
        Task<ProductDomain?> GetByIdAsync(int id);

        Task<bool> CategoryExistsAsync(int CategoryId);
        Task<bool> SupplierExistsAsync(int SupplierId);
        Task<ProductDomain?> UpdateAsync(int id, ProductUpdateDto productDto);
        // ... existing methods
        Task<bool> ImageExistsAsync(int imageId);
    }
}
