using System.ComponentModel.DataAnnotations;
using WebApplicationProductAPI.Models.Domain;

public class ProductDomain
{
    [Key]
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Qty { get; set; }
    public string? Description { get; set; }

    // ✅ Foreign key properties
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }

    // ✅ Navigation properties
    

    public int? ImageId { get; set; }
    public CategoryDomain Category { get; set; }
    public SupplierDomain Supplier { get; set; }

    // Relationships
    
   
                                              // One-to-many relationship: product can have multiple images
    public ICollection<ImageDomain> Images { get; set; } = new List<ImageDomain>();
    public ICollection<OrderItemDomain> OrderItems { get; set; }
    public ICollection<CartItemDomain> CartItems { get; set; }
    public DateTime CreatedDate { get; set; } // ✅ Add this
}
