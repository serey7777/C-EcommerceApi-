using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApplicationProductAPI.Models.Domain
{
    public class CategoryDomain
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        // Additional properties can be added as needed
        [JsonIgnore] // Prevents cycle
        public ICollection<ProductDomain> Products { get; set; } = new List<ProductDomain>();
    }
}
