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
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Qty = p.Qty,
                    Description = p.Description,
                    ImageId = p.ImageId ?? 0,
                    ImagePath = p.Image?.FilePath, // Assuming ImageDomain has Url property
                                                   // Map navigation properties to DTOs
                    Category = p.Category != null ? new CategoryDto
                    {
                        Id = p.Category.Id,
                        Name = p.Category.Name,
                        // Add other CategoryDto properties as needed
                    } : null,
                    Supplier = p.Supplier != null ? new SupplierDto
                    {
                        Id = p.Supplier.Id,
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
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // ✅ Manual mapping for single product (can use same AllProductDto or create ProductDto)
            var response = new AllProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Qty = product.Qty,
                Description = product.Description,
                ImageId = product.ImageId ?? 0,
                ImagePath = product.Image?.FilePath,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                    Description = product.Category.Description,
                    CreatedDate = product.Category.CreatedDate,
                    UpdatedDate = product.Category.UpdatedDate
                } : null,
                Supplier = product.Supplier != null ? new SupplierDto
                {
                    Id = product.Supplier.Id,
                    Name = product.Supplier.Name,
                    ContactEmail = product.Supplier.ContactEmail,
                    PhoneNumber = product.Supplier.PhoneNumber,
                    CreatedDate = product.Supplier.CreatedDate,
                    UpdatedDate = product.Supplier.UpdatedDate
                } : null
            };

            return Ok(response);
        }


        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateProduct([FromBody] AddProductDto addProductDto)
        {
            // Validate CategoryId and SupplierId
            var isCategoryValid = await productRepository.CategoryExistsAsync(addProductDto.category_id);
            var isSupplierValid = await productRepository.SupplierExistsAsync(addProductDto.supplier_id);

            if (!isCategoryValid)
            {
                return BadRequest($"CategoryId {addProductDto.category_id} does not exist.");
            }
            if (!isSupplierValid)
            {
                return BadRequest($"SupplierId {addProductDto.supplier_id} does not exist.");
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
                category_id = addProductDto.category_id,
                supplier_id = addProductDto.supplier_id,
                ImageId = addProductDto.ImageId,  // ✅ Add ImageId (nullable is fine)
                CreatedDate = DateTime.UtcNow
            };

            var addedProduct = await productRepository.AddProductAsync(productDomain);

            var responseDto = new AddProductDto
            {
                Id = addedProduct.Id,
                Name = addedProduct.Name,
                Description = addedProduct.Description,
                Price = addedProduct.Price,
                Qty = addedProduct.Qty,
                category_id = addedProduct.category_id,
                supplier_id = addedProduct.supplier_id,
                ImageId = addedProduct.ImageId // ✅ Include ImageId in response
            };

            return Ok(responseDto);
        }
        //update
        [HttpPut("{id}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto productDto)
        {
            if (!await productRepository.CategoryExistsAsync(productDto.category_id))
                return BadRequest("Invalid Category ID.");

            if (!await productRepository.SupplierExistsAsync(productDto.supplier_id))
                return BadRequest("Invalid Supplier ID.");

            var updatedProduct = await productRepository.UpdateAsync(id, productDto);

            if (updatedProduct == null)
                return NotFound("Product not found.");

            return Ok(updatedProduct); // Consider mapping to ProductDto for cleaner response
        }

        [HttpDelete("{id}")]
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
