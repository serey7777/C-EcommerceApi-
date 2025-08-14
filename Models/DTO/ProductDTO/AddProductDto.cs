namespace WebApplicationProductAPI.Models.DTO.ProductDTO
{
    
        public class AddProductDto
        {
            public int Id { get; internal set; }
            public string? Name { get; set; } = string.Empty;
            public decimal? Price { get; set; }
            public int? Qty { get; set; }
            public string? Description { get; set; }
            public int CategoryId { get; set; }
            public int SupplierId { get; set; }
              public int? ImageId { get; set; } // Added ImageId

    }

    
}
