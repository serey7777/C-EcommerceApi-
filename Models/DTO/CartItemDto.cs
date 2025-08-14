namespace WebApplicationProductAPI.Models.DTO
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public ProductDto Product { get; set; } = null!;
    }
}
