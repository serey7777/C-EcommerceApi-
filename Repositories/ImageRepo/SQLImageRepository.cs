using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;

namespace WebApplicationProductAPI.Repositories.ImageRepo
{
    public class SQLImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SQLImageRepository(
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContext,
            ApplicationDbContext _DbContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContext;
            this.dbContext = _DbContext;
        }

        public async Task<ImageDomain> Upload(ImageDomain image)
        {
            if (image == null || image.File == null)
                throw new ArgumentNullException(nameof(image), "Image or File is null.");

            if (string.IsNullOrWhiteSpace(image.FileName))
                throw new ArgumentNullException(nameof(image.FileName));

            if (string.IsNullOrWhiteSpace(image.FileExtension))
                throw new ArgumentNullException(nameof(image.FileExtension));

            // ✅ Check if ProductId exists
            var productExists = await dbContext.Products
                                   .AnyAsync(p => p.ProductId == image.ProductId);
            if (!productExists)
                throw new InvalidOperationException("ProductId does not exist.");

            var rootPath = webHostEnvironment.WebRootPath
                           ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var imagesFolder = Path.Combine(rootPath, "Images");
            if (!Directory.Exists(imagesFolder))
                Directory.CreateDirectory(imagesFolder);

            var localFilePath = Path.Combine(imagesFolder,
                $"{image.FileName}{image.FileExtension}");

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            image.FilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://" +
                             $"{httpContextAccessor.HttpContext.Request.Host}" +
                             $"{httpContextAccessor.HttpContext.Request.PathBase}/Images/" +
                             $"{image.FileName}{image.FileExtension}";

            await dbContext.Images.AddAsync(image);
            await dbContext.SaveChangesAsync();

            return image;
        }


    }
}
