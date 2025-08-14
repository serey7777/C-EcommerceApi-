using Microsoft.EntityFrameworkCore;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.CategoryDTO;

namespace WebApplicationProductAPI.Repositories.CategoryRepo
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SQLCategoryRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<CategoryDomain>> GetAllAsync()
        {
            return await dbContext.Categories.ToListAsync();
        }

        public async Task<CategoryDomain?> GetCategoryAsync(int id)
        {
            return await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
        }

        public async Task<CategoryDomain> AddPostAsync(CategoryDomain category)
        {
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();
            return category;
        }

        public async Task<CategoryDomain?> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
        {
            var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingCategory == null)
                return null;

            // Update properties based on your CategoryUpdateDto
            existingCategory.Name = dto.Name;
            existingCategory.Description = dto.Description;
            // Add other properties as needed

            await dbContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<CategoryDomain?> DeleteCategoryAsync(int id)
        {
            var existingCategory = await dbContext.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (existingCategory == null)
                return null;

            dbContext.Categories.Remove(existingCategory);
            await dbContext.SaveChangesAsync();
            return existingCategory;
        }
    }
}
