using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO;
using WebApplicationProductAPI.Models.DTO.CategoryDTO;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

namespace WebApplicationProductAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Require user login
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [HttpGet("debug")]
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            return Ok(claims);
        }


        // GET: api/cart
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetCart()
        {
            try
            {
                // Get the current user
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                // Load the cart with related data
                var cart = await _context.Carts
    .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Images) // Load multiple images
    .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Category)
    .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
            .ThenInclude(p => p.Supplier)
    .FirstOrDefaultAsync(c => c.UserId == user.Id);


                // If the cart does not exist, create a new one
                if (cart == null)
                {
                    cart = new CartDomain
                    {
                        UserId = user.Id,
                        CartItems = new List<CartItemDomain>()
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                // Map to DTO
                var cartDto = new CartDto
                {
                    CartId = cart.CartId,
                    Items = cart.CartItems.Select(ci => new CartItemDto
                    {
                        CartItemId = ci.CartItemId,
                        Quantity = ci.Quantity,
                        Product = new ProductDto
                        {
                            Id = ci.Product.ProductId,
                            Name = ci.Product.Name,
                            Price = ci.Product.Price,
                            Qty = ci.Product.Qty,
                            Description = ci.Product.Description,
                            CategoryId = ci.Product.CategoryId,
                            SupplierId = ci.Product.SupplierId,
                            Category = ci.Product.Category != null ? new CategoryDto
                            {
                                Id = ci.Product.Category.CategoryId,
                                Name = ci.Product.Category.Name,
                                Description = ci.Product.Category.Description,
                                CreatedDate = ci.Product.Category.CreatedDate,
                                UpdatedDate = ci.Product.Category.UpdatedDate
                            } : null,
                            Supplier = ci.Product.Supplier != null ? new SupplierDto
                            {
                                Id = ci.Product.Supplier.SupplierId,
                                Name = ci.Product.Supplier.Name
                            } : null,
                            Images = ci.Product.Images != null ? ci.Product.Images.Select(img => img.FileName).ToList() : new List<string>(),
                            ImagePath = ci.Product.Images != null ? ci.Product.Images.Select(img => img.FilePath).ToList() : new List<string>()
                        }
                    }).ToList()
                };



                // Return the DTO
                return Ok(cartDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        // POST: api/cart/add
        [HttpPost("add")]
        public async Task<ActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                // Check if product exists
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null)
                    return NotFound("Product not found");

                // Check if product is in stock
                if (product.Qty < request.Quantity)
                    return BadRequest($"Insufficient stock. Available: {product.Qty}");

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (cart == null)
                {
                    cart = new CartDomain
                    {
                        UserId = user.Id,
                        CartItems = new List<CartItemDomain>()
                    };
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

                if (existingCartItem != null)
                {
                    // Check total quantity doesn't exceed stock
                    var totalQuantity = existingCartItem.Quantity + request.Quantity;
                    if (totalQuantity > product.Qty)
                        return BadRequest($"Total quantity ({totalQuantity}) exceeds available stock ({product.Qty})");

                    existingCartItem.Quantity = totalQuantity;
                    _context.CartItems.Update(existingCartItem);
                }
                else
                {
                    var newCartItem = new CartItemDomain
                    {
                        CartId = cart.CartId,
                        ProductId = request.ProductId,
                        Quantity = request.Quantity
                    };
                    _context.CartItems.Add(newCartItem);
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Item added to cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/cart/update
        [HttpPut("update")]
        public async Task<ActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (cart == null) return NotFound("Cart not found");

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
                if (cartItem == null) return NotFound("Item not in cart");

                // Check stock availability
                var product = await _context.Products.FindAsync(request.ProductId);
                if (product == null) return NotFound("Product not found");

                if (request.Quantity > product.Qty)
                    return BadRequest($"Requested quantity ({request.Quantity}) exceeds available stock ({product.Qty})");

                cartItem.Quantity = request.Quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart item updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/cart/remove
        [HttpDelete("remove")]
        public async Task<ActionResult> RemoveFromCart([FromBody] RemoveFromCartRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (cart == null) return NotFound("Cart not found");

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
                if (cartItem == null) return NotFound("Item not in cart");

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Item removed from cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/cart/clear
        [HttpDelete("clear")]
        public async Task<ActionResult> ClearCart()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (cart == null) return NotFound("Cart not found");

                if (cart.CartItems.Any())
                {
                    _context.CartItems.RemoveRange(cart.CartItems);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Cart cleared successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/cart/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartItemCount() 
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var itemCount = await _context.CartItems
                    .Where(ci => ci.Cart.UserId == user.Id)
                    .SumAsync(ci => ci.Quantity);

                return Ok(new { count = itemCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

