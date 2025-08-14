using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationProductAPI.Models.Domain
{
    public class OrderDomain
    {
        [Key]
        public int OrderId { get; set; }

        // Link to Identity User
        public string UserId { get; set; } = null!; // FK to AspNetUsers.Id


        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation
        public ICollection<OrderItemDomain> OrderItems { get; set; } = new List<OrderItemDomain>();

    }
    public enum OrderStatus
    {
        Pending,
        Paid,
        Shipped,
        Cancelled
    }
}
