using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Repositories.CategoryRepo;

namespace WebApplicationProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ICategoryRepository categoryRepository;
        private readonly ILogger<CategoryController> logger;

        public CategoryController(ApplicationDbContext dbContext, ICategoryRepository categoryRepository, ILogger<CategoryController> logger)
        {
            this.dbContext = dbContext;
            this.categoryRepository = categoryRepository;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation($"Get all categories action method invoked");
            var categories = await categoryRepository.GetAllAsync();
            var response = categories.Adapt<List<CategoryDto>>();
            return Ok(response);
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await categoryRepository.GetCategoryAsync(id);
            if (category == null)
                return NotFound("Category Not Found");
            
            var response = category.Adapt<CategoryDto>();
            return Ok(response);
        }

        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> AddPost([FromBody] CategoryAddDto dto)
        {
            var category = dto.Adapt<CategoryDomain>();
            category = await categoryRepository.AddPostAsync(category);
            var response = category.Adapt<CategoryAddDto>();
            return CreatedAtAction(nameof(GetCategory), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDto dto)
        {
            var updatedCategory = await categoryRepository.UpdateCategoryAsync(id, dto);
            if (updatedCategory == null)
            {
                return NotFound("Category Not Found");
            }

            dto.Adapt(updatedCategory);
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await categoryRepository.DeleteCategoryAsync(id);
            if (category == null)
                return NotFound("Category Not Found In Database");

            var response = category.Adapt<CategoryDomain>();
            return Ok(response);
        }
    }
}
