using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationProductAPI.Models.Domain
{
    public class CartDomain
    {
        [Key]
        public int CartId { get; set; }

        public string UserId { get; set; } = null!; // FK to AspNetUsers.Id
        

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<CartItemDomain> CartItems { get; set; } = new List<CartItemDomain>();
    }
}