using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

public class ProductDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public int? Qty { get; set; }
    public string? Description { get; set; }

    public int category_id { get; set; }
    public int supplier_id { get; set; }

    // Optional navigation properties (if needed in response)
    public CategoryDto? Category { get; set; }
    public SupplierDto? Supplier { get; set; }
    public int? ImageId { get; set; } //  Added ImageId
}
