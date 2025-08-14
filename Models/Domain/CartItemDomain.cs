using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplicationProductAPI.Models.Domain
{
    public class CartItemDomain
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign Keys
        public int CartId { get; set; }
        public int ProductId { get; set; }

        // Navigation
        [JsonIgnore] // Prevents cycle
        public CartDomain Cart { get; set; } = null!;
        public ProductDomain Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
