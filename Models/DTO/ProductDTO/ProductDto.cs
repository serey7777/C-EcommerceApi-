using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

public class ProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int? Qty { get; set; }
    public string? Description { get; set; }

    public int CategoryId { get; set; }
    public int SupplierId { get; set; }

    // Optional navigation properties (if needed in response)
    public CategoryDto? Category { get; set; }
    public SupplierDto? Supplier { get; set; }
    public int? ImageId { get; set; } //  Added ImageId
                                      // Add this for multiple image URLs
    public List<string> Images { get; set; } = new List<string>();
    public List<string> ImagePath { get; set; } = new List<string>();
}
