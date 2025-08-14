using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;

namespace WebApplicationProductAPI.Controllers
{
    // Input Models
    public class AddToCartRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class RemoveFromCartRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }
    }

    public class UpdateCartItemRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

   

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDomain>>> GetOrders()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Images)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Category)
                    .Where(o => o.UserId == user.Id)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDomain>> GetOrder(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Images)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == user.Id);

                if (order == null) return NotFound("Order not found");

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/orders/create
        [HttpPost("create")]
        public async Task<ActionResult<OrderDomain>> CreateOrder()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

                if (cart == null || !cart.CartItems.Any())
                    return BadRequest("Cart is empty");

                // Validate stock availability for all items
                var stockValidationErrors = new List<string>();
                foreach (var cartItem in cart.CartItems)
                {
                    if (cartItem.Product.Qty < cartItem.Quantity)
                    {
                        stockValidationErrors.Add($"Insufficient stock for {cartItem.Product.Name}. Available: {cartItem.Product.Qty}, Requested: {cartItem.Quantity}");
                    }
                }

                if (stockValidationErrors.Any())
                {
                    return BadRequest(new { errors = stockValidationErrors });
                }

                // Calculate total amount
                var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

                var order = new OrderDomain
                {
                    UserId = user.Id,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = totalAmount,
                    OrderItems = cart.CartItems.Select(ci => new OrderItemDomain
                    {
                        ProductId = ci.ProductId,
                        Quantity = ci.Quantity,
                        Price = ci.Product.Price
                    }).ToList()
                };

                _context.Orders.Add(order);

                // Update product stock
                foreach (var cartItem in cart.CartItems)
                {
                    cartItem.Product.Qty -= cartItem.Quantity;
                    _context.Products.Update(cartItem.Product);
                }

                // Clear cart
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Reload order with full details
                var createdOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Images)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Category)
                    .FirstOrDefaultAsync(o => o.OrderId == order.OrderId);

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, createdOrder);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/orders/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized("User not found");

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == user.Id);

                if (order == null) return NotFound("Order not found");

                if (order.Status != OrderStatus.Pending)
                    return BadRequest("Only pending orders can be cancelled");

                // Restore product stock
                foreach (var orderItem in order.OrderItems)
                {
                    orderItem.Product.Qty += orderItem.Quantity;
                    _context.Products.Update(orderItem.Product);
                }

                order.Status = OrderStatus.Cancelled;
                _context.Orders.Update(order);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}