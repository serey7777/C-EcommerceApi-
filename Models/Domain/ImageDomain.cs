using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WebApplicationProductAPI.Models.Domain;

public class ImageDomain
{
    [Key]
    public int Id { get; set; }

    [NotMapped]
    public IFormFile File { get; set; }

    [Required]
    [MaxLength(255)]
    public string FileName { get; set; }

    public string? FileDescription { get; set; }

    [Required]
    [MaxLength(10)]
    public string FileExtension { get; set; }

    public long FileSizeInBytes { get; set; }

    [Required]
    [MaxLength(500)]
    public string FilePath { get; set; }
}
