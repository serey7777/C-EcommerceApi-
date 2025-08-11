using System.ComponentModel.DataAnnotations;

namespace WebApplicationProductAPI.Models.DTO.ImagesDTO
{
    public class ImageUpdateRequestDto
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FileName{ get; set; }
        public string? FileDescription { get; set; }
    }
}
