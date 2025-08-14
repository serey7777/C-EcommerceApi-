using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using WebApplicationProductAPI.Models.Domain;

namespace WebApplicationProductAPI.DataConn
{
    // Main DbContext: Identity + Domain tables
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Domain tables
        public DbSet<ProductDomain> Products { get; set; } = null!;
        public DbSet<CategoryDomain> Categories { get; set; } = null!;
        public DbSet<SupplierDomain> Suppliers { get; set; } = null!;
        public DbSet<ImageDomain> Images { get; set; } = null!;
        public DbSet<OrderDomain> Orders { get; set; } = null!;
        public DbSet<OrderItemDomain> OrderItems { get; set; } = null!;
        public DbSet<CartDomain> Carts { get; set; } = null!;
        public DbSet<CartItemDomain> CartItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Must call for Identity

            // ================= Role Seeding =================
            var readerId = "c8ce66f0-93fa-493b-8b0a-33863f13cf26";
            var writerId = "ad2bd6ef-8cc3-4ca3-8df9-40d1ce832b36";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = readerId,
                    ConcurrencyStamp = readerId,
                    Name = "Reader",
                    NormalizedName = "READER"
                },
                new IdentityRole
                {
                    Id = writerId,
                    ConcurrencyStamp = writerId,
                    Name = "Writer",
                    NormalizedName = "WRITER"
                }
            );

            // ================= Relationships =================

            // Cart - User (one-to-one/many)
            modelBuilder.Entity<CartDomain>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - User
            modelBuilder.Entity<OrderDomain>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Category - Product (one-to-many)
            modelBuilder.Entity<CategoryDomain>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Supplier - Product (one-to-many)
            modelBuilder.Entity<SupplierDomain>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - OrderItem (one-to-many)
            modelBuilder.Entity<ProductDomain>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - OrderItem (one-to-many)
            modelBuilder.Entity<OrderDomain>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart - CartItem (one-to-many)
            modelBuilder.Entity<CartDomain>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product - CartItem (one-to-many)
            modelBuilder.Entity<ProductDomain>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Image (one-to-many)
            modelBuilder.Entity<ProductDomain>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    // Factory for Design-Time Migrations
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
