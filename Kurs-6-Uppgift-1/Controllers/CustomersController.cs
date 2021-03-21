using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kurs_6_Uppgift_1.Data;
using Kurs_6_Uppgift_1.Services;

namespace Kurs_6_Uppgift_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly SqlDbContext _context;
        private readonly IIdentityService _identityService;

        public CustomersController(SqlDbContext context, IIdentityService identityService)
        {
            _context = context;
            _identityService = identityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetSpecificCustomer(int id)
        {
            if (_context.Customers.Count() < id)
            {
                return new BadRequestObjectResult("Customer does not exist");
            }
            else
                return new OkObjectResult(await _identityService.GetSpecificCustomer(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer model)
        {
            if (await _identityService.CreateCustomerAsync(model))
                return new OkObjectResult("Customer has been created");

            return new BadRequestObjectResult("Email already exists!");
        }
    }
}
