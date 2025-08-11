using Azure;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.SupplierDTO;
using WebApplicationProductAPI.Repositories.SupplierRepo;

namespace WebApplicationProductAPI.Controllers
{
//Client       →   Server
// Login       →   Check password
//             ←   Send token

//Client       →   Send token in Authorization header
//Server       →   Validate token
//             ←   Allow or deny access

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ISupplierRepository supplierRepository;
        private readonly ILogger<SupplierController> logger; // Change type

        public SupplierController(ApplicationDbContext dbContext, ISupplierRepository supplierRepository, ILogger<SupplierController> logger) // Change type
        {
            this.dbContext = dbContext;
            this.supplierRepository = supplierRepository;
            this.logger = logger;
        }

        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("GetAll() method was invoked by user: {User}", HttpContext.User.Identity?.Name);

            var suppliers = await supplierRepository.GetAllAsync();
            var response = suppliers.Adapt<List<SupplierDto>>();

            return Ok(response);
        }


        [HttpGet("{id}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetSupplier(int id)
        {
            var supplier = await supplierRepository.GetSupplierAsync(id);
            if (supplier == null)
                return NotFound("Supplier Not Found");
            //Mapping supplier to Supplier STO
            var response = supplier.Adapt<SupplierDto>();

            return Ok(response);
        }
        [HttpPost]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> AddPost([FromBody] SupplierAddDto dto)
        {
            //pass data from dto to domain
            var supplier = dto.Adapt<SupplierDomain>();

            supplier = await supplierRepository.AddPostAsync(supplier);
            //supplierDomain to DTO for show client
            var respone = supplier.Adapt<SupplierAddDto>();
            return CreatedAtAction(nameof(GetSupplier), new { id = respone.Id }, respone);
        }
        [HttpPut("{id}")]
        // [Authorize(Roles = "Writer")]  // Uncomment if you want role-based auth
        public async Task<SupplierDomain> UpdateSupplierAsync(int id, SupplierUpdateDto dto)
        {
            // 1. Load existing supplier from DB
            var supplier = await dbContext.Suppliers.FindAsync(id);
            if (supplier == null)
                return null;

            // 2. Update only non-key properties
            supplier.Name = dto.Name;
            supplier.PhoneNumber = dto.PhoneNumber;
            supplier.ContactEmail = dto.ContactEmail;
            // ... update other fields as needed

            // 3. Save changes
            await dbContext.SaveChangesAsync();

            return supplier;
        }


        [HttpDelete("{id}")]
        //[Authorize(Roles = "Writer, Reader")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            var supplier = await supplierRepository.DeleteSupplierAsync(id);
            if (supplier == null)
                return NotFound("Supplier Not Found In Database");

            //domain to dto 
            var respone = supplier.Adapt<SupplierDomain>();

            return Ok(respone);
        }

    }
}
