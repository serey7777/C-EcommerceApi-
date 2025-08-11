namespace WebApplicationProductAPI.Models.DTO.CategoryDTO
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; internal set; }
        public DateTime UpdatedDate { get; internal set; }
    }
}
