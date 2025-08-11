using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.ImagesDTO;
using WebApplicationProductAPI.Repositories.ImageRepo;

namespace WebApplicationProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepo;

        public ImagesController(IImageRepository  imageRepo)
        {
            this.imageRepo = imageRepo;
        }
        //POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUpdateRequestDto request)
        {
            ValidateFileUpdate(request);

            if (ModelState.IsValid)
            {
                var imageDomainModel = new ImageDomain
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription
                };

                await imageRepo.Upload(imageDomainModel);
                return Ok(imageDomainModel);
            }
            return BadRequest(ModelState);
        }
        private void ValidateFileUpdate(ImageUpdateRequestDto request)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var ext = Path.GetExtension(request.File.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
            {
                ModelState.AddModelError("file", "Unsupported images extension");
            }

            if (request.File.Length > 10 * 1024 * 1024)
            {
                ModelState.AddModelError("file", "File size more than 10MB, please upload smaller file");
            }
        }

    }
}
