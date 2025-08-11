using Mapster;
using Microsoft.EntityFrameworkCore;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;

namespace WebApplicationProductAPI.Repositories.SupplierRepo
{
    public class SQLSupplierRepository : ISupplierRepository
    {
        private readonly ApplicationDbContext dbContext;

        public SQLSupplierRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<SupplierDomain> AddPostAsync(SupplierDomain supplierDomain)
        {
            await dbContext.Suppliers.AddAsync(supplierDomain);
            await dbContext.SaveChangesAsync();

            return supplierDomain;
        }

        public async Task<SupplierDomain?> DeleteSupplierAsync(int id)
        {
            var existingSupplier = await dbContext.Suppliers.FindAsync(id);
            if (existingSupplier == null)
            {
                return null;
            }

            dbContext.Suppliers.Remove(existingSupplier);
            await dbContext.SaveChangesAsync();

            return existingSupplier;
        }

        public async Task<List<SupplierDomain>> GetAllAsync()
        {
            return await dbContext.Suppliers.ToListAsync();
        }

        public async Task<SupplierDomain>? GetSupplierAsync(int id)
        {
            return await dbContext.Suppliers.FindAsync(id);
        }

        public async Task<SupplierDomain?> UpdateSupplierAsync(int id, SupplierUpdateDto supplierUpdate)
        {
            var existingSupplier = await dbContext.Suppliers.FindAsync(id);

            if (existingSupplier == null)
            {
                return null;
            }

            supplierUpdate.Adapt(existingSupplier);

            await dbContext.SaveChangesAsync();

            return existingSupplier;
        }
    }
}
