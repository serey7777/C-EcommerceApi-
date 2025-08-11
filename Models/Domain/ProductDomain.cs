using WebApplicationProductAPI.Models.Domain;

public class ProductDomain
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public string? Description { get; set; }

    // ✅ Foreign key properties
    public int category_id { get; set; }
    public int supplier_id { get; set; }

    // ✅ Navigation properties
    public CategoryDomain Category { get; set; }
    public SupplierDomain Supplier { get; set; }

    public int? ImageId { get; set; }
    public ImageDomain Image { get; set; }
    public DateTime CreatedDate { get; set; } // ✅ Add this
   
}
