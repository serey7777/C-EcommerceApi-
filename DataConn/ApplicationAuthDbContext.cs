using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationProductAPI.DataConn
{
    public class ApplicationAuthDbContext : IdentityDbContext
    {
        public ApplicationAuthDbContext(DbContextOptions<ApplicationAuthDbContext> options) : base(options)
        {
        }

        protected ApplicationAuthDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerId = "c8ce66f0-93fa-493b-8b0a-33863f13cf26";
            var writerId = "ad2bd6ef-8cc3-4ca3-8df9-40d1ce832b36";

            var roles = new List<IdentityRole>
            {
              new IdentityRole
              { 
                Id = readerId,
                ConcurrencyStamp = readerId,
                Name= "Reader",
                NormalizedName = "Reader".ToUpper()
              },
              new IdentityRole
              { 
                Id =writerId,
                ConcurrencyStamp = writerId,
                Name = "Writer",
                NormalizedName = "Writer".ToUpper()
              }

            };
            builder.Entity<IdentityRole>().HasData(roles);
        }

    }
}
