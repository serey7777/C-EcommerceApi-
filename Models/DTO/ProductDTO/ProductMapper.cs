using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

namespace WebApplicationProductAPI.Models.DTO.ProductDTO
{
    public class ProductMapper
    {
        public static ProductDto MapToDto(ProductDomain product)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
            return new ProductDto
            {
               
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                Description = product.Description,


                Category = product.Category == null ? null : new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description,
                    CreatedDate = product.Category.CreatedDate,
                    UpdatedDate = product.Category.UpdatedDate
                },
                Supplier = product.Supplier == null ? null : new SupplierDto
                {
                    Id = product.Supplier.Id,
                    Name = product.Supplier.Name,
                    ContactEmail = product.Supplier.ContactEmail,
                    PhoneNumber = product.Supplier.PhoneNumber,
                    CreatedDate = product.Supplier.CreatedDate,
                    UpdatedDate = product.Supplier.UpdatedDate
                }
            };
#pragma warning restore CS8601 // Possible null reference assignment.
        }
    }
}
