using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.CategoryDTO;

namespace WebApplicationProductAPI.Repositories.CategoryRepo
{
    public interface ICategoryRepository
    {
        Task<List<CategoryDomain>> GetAllAsync();
        Task<CategoryDomain?> GetCategoryAsync(int id);
        Task<CategoryDomain> AddPostAsync(CategoryDomain category);
        Task<CategoryDomain?> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
        Task<CategoryDomain?> DeleteCategoryAsync(int id);
    }
}
