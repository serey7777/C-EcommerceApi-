using System.ComponentModel.DataAnnotations;

namespace WebApplicationProductAPI.Models.DTO.CategoryDTO
{
    public class CategoryUpdateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        // Add other properties as needed
    }
}