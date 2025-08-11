using WebApplicationProductAPI.Models.Domain;

namespace WebApplicationProductAPI.Repositories.ImageRepo
{
    public interface IImageRepository
    {
        Task<ImageDomain> Upload(ImageDomain image);
    }
}
