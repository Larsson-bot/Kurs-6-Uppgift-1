using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kurs_6_Uppgift_1.Data;
using Kurs_6_Uppgift_1.Services;
using Microsoft.AspNetCore.Authorization;

namespace Kurs_6_Uppgift_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CasesController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly SqlDbContext _context;

        public CasesController(IIdentityService identityService, SqlDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCases()
        {
            return new OkObjectResult(await _identityService.GetCases());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Case>> GetCase(int id)
        {
            var @case = await _context.Cases.FindAsync(id);

            if (@case == null)
            {
                return NotFound();
            }
            return @case;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCase(int id, Case updatedCase)
        {
            updatedCase.Id = id;
            var date = DateTime.Now;
            updatedCase.LatestChange = DateTime.Now;
            _context.Entry(updatedCase).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CaseExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("create-case")]
        public async Task<IActionResult> CreateCase([FromBody] Case model)
        {
            var customer = _context.Customers.FirstOrDefault(x => x.Id == model.CustomerId);
            if (customer != null)
            {
                if (await _identityService.CreateCaseAsync(model))
                    return new OkResult();

                return new BadRequestResult();
            }
            return new NotFoundObjectResult("Customer was not found.");
           
        }

        [HttpGet("getonstatus")]
        public async Task<IActionResult> GetOnStatus(string status)
        {
            var list = await _context.Cases.Where(x => x.Status == status).ToListAsync();
            if (list.Count() != 0)
            {

                foreach (var @case in list)
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == @case.CustomerId);

                    @case.Customer = customer;
                }
                return new OkObjectResult(list);
            }
            return new OkObjectResult(list);
        }

        [HttpGet("getoncustomerid")]
        public async Task<IActionResult> GetCasesOnCustomerId(int id)
        {
            var list = await _context.Cases.Where(x => x.CustomerId == id).ToListAsync();
            if (list.Count() != 0)
            {

                foreach (var @case in list)
                {
                    var customer = _context.Customers.FirstOrDefault(c => c.Id == @case.CustomerId);

                    @case.Customer = customer;
                }
                return new OkObjectResult(list);
            }
            return new OkObjectResult(list);
        }

        [HttpGet("getondate")]
        public async Task<IActionResult> GetCasesOnSortedDate(string ordervalue)
        {
            var list =  await _context.Cases.ToListAsync();
            foreach (var @case in list)
            {
                var customer = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Id == @case.CustomerId);
                @case.Customer = customer;
            }

            if (ordervalue.ToLower() == "latest")
            {
                var anotherlist =  list.OrderByDescending(c => c.Created);
                return new OkObjectResult(anotherlist);
            }

            if (ordervalue.ToLower() == "oldest")
            {
                var anotherlist= list.OrderBy(s => s.Created);
                return new OkObjectResult(anotherlist);
            }
            return new OkObjectResult(list); 
        }

        private bool CaseExists(int id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }
    }
}
