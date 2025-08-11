namespace WebApplicationProductAPI.Models.DTO.SupplierDTO
{
    public class SupplierUpdateDto
    {
        public int Id { get; set; } // Required to identify which supplier to update
        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
    }
}
