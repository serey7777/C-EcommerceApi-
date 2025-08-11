namespace WebApplicationProductAPI.Models.Domain
{
    public class SupplierDomain
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
        // Additional properties can be added as needed
        public ICollection<ProductDomain> Products { get; set; } = new List<ProductDomain>();
        
    }
}
