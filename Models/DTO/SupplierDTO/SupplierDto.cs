
namespace WebApplicationProductAPI.Models.DTO.SupplierDTO
{
    public class SupplierDto
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string ContactEmail { get; set; } 
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; internal set; }
        public DateTime UpdatedDate { get; internal set; }
    }
}
