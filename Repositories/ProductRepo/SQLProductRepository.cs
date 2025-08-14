using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.ProductDTO;

namespace WebApplicationProductAPI.Repositories.ProductRepo
{

    public class SQLProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<SQLProductRepository> _logger;

        public SQLProductRepository(ApplicationDbContext dbContext, ILogger<SQLProductRepository> logger) 
        {
            this.dbContext = dbContext;
            _logger = logger;
        }
        //Add Product
        public async Task<ProductDomain> AddProductAsync(ProductDomain productDomain)
        {
            await dbContext.Products.AddAsync(productDomain);
            await dbContext.SaveChangesAsync();

            return productDomain;
        }

        // Show all Products
        public async Task<List<ProductDomain>> GetAllProductAsync(string? filterOn = null, string? filterQuery = null)
        {
            try
            {
                _logger.LogInformation("GetAllProductAsync called");

                var products = dbContext.Products
                    .Include(p => p.Category)
                    .Include(p => p.Supplier)
                    .Include(p => p.Images)
                    .AsQueryable();

                _logger.LogInformation("Base query created");

                if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
                {
                    if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                    {
                        products = products.Where(x => x.Name.Contains(filterQuery));
                        _logger.LogInformation("Applied name filter: {FilterQuery}", filterQuery);
                    }
                }

                var result = await products.ToListAsync();
                _logger.LogInformation("Query executed successfully, returned {Count} products", result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllProductAsync");
                throw; // Re-throw to let controller handle it
            }
        }

        //get category ID
        public async Task<bool> CategoryExistsAsync(int categoryId)
        {
            return await dbContext.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }
        //get supplier ID
        public async Task<bool> SupplierExistsAsync(int supplierId)
        {
            return await dbContext.Suppliers.AnyAsync(s => s.SupplierId == supplierId);
        }
        //show by ID
        public async Task<ProductDomain?> GetByIdAsync(int id)
        {
            return await dbContext.Products.Include(p => p.Category).Include(p => p.Supplier).FirstOrDefaultAsync(p => p.ProductId == id);
        }

        //update by id
        public async Task<ProductDomain?> UpdateAsync(int id, ProductUpdateDto productDto)
        {
            var existingProduct = await dbContext.Products.FindAsync(id);

            if (existingProduct == null)
                return null;

            // Update product properties
            existingProduct.Name = productDto.Name ?? existingProduct.Name;
            existingProduct.Price = productDto.Price ?? existingProduct.Price;
            existingProduct.Qty = productDto.Qty ?? existingProduct.Qty;
            existingProduct.Description = productDto.Description ?? existingProduct.Description;

            // Update foreign keys
            existingProduct.CategoryId = productDto.CategoryId;
            existingProduct.SupplierId = productDto.SupplierId;

            await dbContext.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<bool> ImageExistsAsync(int imageId)
        {
            return await dbContext.Images.AnyAsync(i => i.Id == imageId);
        }
    }
}
