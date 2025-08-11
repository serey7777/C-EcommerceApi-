using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

namespace WebApplicationProductAPI.Models.DTO.ProductDTO
{
    public class AllProductDto
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public int? Qty { get; set; }
        public string? Description { get; set; }

        public string ImagePath { get; set; }

        // Optional navigation properties (if needed in response)
        // this show all info and data in cateory and supplier
        public CategoryDto? Category { get; set; }
        public SupplierDto? Supplier { get; set; }
        public int ImageId { get; set; } //  Added ImageId
    }
}
