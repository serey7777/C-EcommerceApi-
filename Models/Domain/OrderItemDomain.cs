using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationProductAPI.Models.Domain
{
    public class OrderItemDomain
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign Keys
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        // Navigation
        public OrderDomain Order { get; set; } = null!;
        public ProductDomain Product { get; set; } = null!;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

    }
}
