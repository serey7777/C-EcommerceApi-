namespace WebApplicationProductAPI.Models.DTO.ProductDTO
{
    
        public class AddProductDto
        {
            public int Id { get; internal set; }
            public string? Name { get; set; } = string.Empty;
            public decimal? Price { get; set; }
            public int? Qty { get; set; }
            public string? Description { get; set; }
            public int category_id { get; set; }
            public int supplier_id { get; set; }
              public int? ImageId { get; set; } // Added ImageId

    }

    
}
