namespace WebApplicationProductAPI.Models.DTO
{
    public class CartDto
    {
        public int CartId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
    }
}
