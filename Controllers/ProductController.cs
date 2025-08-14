using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.ProductDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;
using WebApplicationProductAPI.Repositories.ProductRepo;

namespace WebApplicationProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IProductRepository productRepository;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ApplicationDbContext dbContext, IProductRepository productRepository, ILogger<ProductController> logger)
        {
            this.dbContext = dbContext;
            this.productRepository = productRepository;
            _logger = logger;
        }

        // Show all product 
        //Get : /api/product?filterOn=Name&filterQuery=Price

      // Controller with correct manual mapping
            //[Authorize(Roles ="Writer, Reader")]
            [HttpGet]
            public async Task<IActionResult> GetAllProduct([FromQuery] string? filterOn, [FromQuery] string? filterQuery)
        {
            try
            {
                _logger.LogInformation("GetAllProduct called with filterOn: {FilterOn}, filterQuery: {FilterQuery}", filterOn, filterQuery);
                var products = await productRepository.GetAllProductAsync(filterOn, filterQuery);
                _logger.LogInformation("Retrieved {Count} products from repository", products.Count);

                // ✅ Manual mapping matching your AllProductDto structure
                var response = products.Select(p => new AllProductDto
                {
                    Id = p.ProductId,
                    Name = p.Name,
                    Price = p.Price,
                    Qty = p.Qty,
                    Description = p.Description,
                    ImageId = p.ImageId ?? 0,
                    ImagePath = p.Images != null
    ? p.Images.Select(img => img.FilePath).ToList()
    : new List<string>(),
                    // Assuming ImageDomain has Url property
                    // Map navigation properties to DTOs
                    Category = p.Category != null ? new CategoryDto
                    {
                        Id = p.Category.CategoryId,
                        Name = p.Category.Name,
                        // Add other CategoryDto properties as needed
                    } : null,
                    Supplier = p.Supplier != null ? new SupplierDto
                    {
                        Id = p.Supplier.SupplierId,
                        Name = p.Supplier.Name,
                        // Add other SupplierDto properties as needed
                    } : null
                }).ToList();

                _logger.LogInformation("Successfully mapped to DTOs");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllProduct with filterOn: {FilterOn}, filterQuery: {FilterQuery}", filterOn, filterQuery);
                return StatusCode(500, new { message = "Server error", detail = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var response = new ProductDto
            {
                Id = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                Description = product.Description,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.CategoryId,
                    Name = product.Category.Name
                } : null,
                Supplier = product.Supplier != null ? new SupplierDto
                {
                    Id = product.Supplier.SupplierId,
                    Name = product.Supplier.Name
                } : null,
                ImageId = product.Images?.FirstOrDefault()?.Id,
                Images = product.Images != null ? product.Images.Select(img => img.FilePath).ToList() : new List<string>(),
                ImagePath = product.Images != null ? product.Images.Select(img => img.FilePath).ToList() : new List<string>()
            };



            return Ok(response);
        }


        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductDto addProductDto)
        {
            // Validate CategoryId and SupplierId
            var isCategoryValid = await productRepository.CategoryExistsAsync(addProductDto.CategoryId);
            var isSupplierValid = await productRepository.SupplierExistsAsync(addProductDto.SupplierId);

            if (!isCategoryValid)
            {
                return BadRequest($"CategoryId {addProductDto.CategoryId} does not exist.");
            }
            if (!isSupplierValid)
            {
                return BadRequest($"SupplierId {addProductDto.SupplierId} does not exist.");
            }

            // ✅ Validate ImageId if provided
            if (addProductDto.ImageId.HasValue)
            {
                var isImageValid = await productRepository.ImageExistsAsync(addProductDto.ImageId.Value);
                if (!isImageValid)
                {
                    return BadRequest($"ImageId {addProductDto.ImageId} does not exist.");
                }
            }

            var productDomain = new ProductDomain
            {
                Name = addProductDto.Name ?? string.Empty, // Handle null
                Description = addProductDto.Description,
                Price = addProductDto.Price ?? 0, // ✅ Handle nullable decimal
                Qty = addProductDto.Qty ?? 0,     // ✅ Handle nullable int
                CategoryId = addProductDto.CategoryId,
                SupplierId = addProductDto.SupplierId,
                ImageId = addProductDto.ImageId,  // ✅ Add ImageId (nullable is fine)
                CreatedDate = DateTime.UtcNow
            };

            var addedProduct = await productRepository.AddProductAsync(productDomain);

            var responseDto = new AddProductDto
            {
                Id = addedProduct.ProductId,
                Name = addedProduct.Name,
                Description = addedProduct.Description,
                Price = addedProduct.Price,
                Qty = addedProduct.Qty,
                CategoryId = addedProduct.CategoryId,
                SupplierId = addedProduct.SupplierId,
                ImageId = addedProduct.ImageId // ✅ Include ImageId in response
            };

            return Ok(responseDto);
        }
        //update
        [HttpPut("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            if (!await productRepository.CategoryExistsAsync(productDto.CategoryId))
                return BadRequest("Invalid Category ID.");

            if (!await productRepository.SupplierExistsAsync(productDto.SupplierId))
                return BadRequest("Invalid Supplier ID.");

            var updatedProduct = await productRepository.UpdateAsync(id, productDto);

            if (updatedProduct == null)
                return NotFound("Product not found.");

            return Ok(updatedProduct); // Consider mapping to ProductDto for cleaner response
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteProduct(int id) 
        {
            var product = await dbContext.Products.FindAsync(id);

            if (product == null) {
                return NotFound("Product not found");
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            return Ok("Pruduct Delete Sucessfully ");
        }

    }
}
