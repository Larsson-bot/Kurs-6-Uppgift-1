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

        // GET: api/Cases
        [HttpGet]
        public async Task<IActionResult> GetCases()
        {

            return new OkObjectResult(await _identityService.GetCases());
        }

        // GET: api/Cases/5
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

        // PUT: api/Cases/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCase(int id, Case updatedCase)
        {
            updatedCase.Id = id;
            if (id != updatedCase.Id)
            {
                return BadRequest();
            }



            var date = DateTime.Now;
            updatedCase.LatestChange = DateTime.Now;
            
            //foreach (var errand in issue)
            //{
            //    updatedCase.AdminstratorId = errand.AdminstratorId;
            //    updatedCase.Created = errand.Created;
            //    updatedCase.CustomerId = errand.CustomerId;
            //    updatedCase.LatestChange = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            //    updatedCase.Description = errand.Description;
            //}

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
            if (_context.Customers.Count() < model.CustomerId)
            {
                return new NotFoundObjectResult("Customer was not found.");
            }

            if (await _identityService.CreateCaseAsync(model))
                return new OkResult();

            return new BadRequestResult();
        }

        [HttpGet("getonstatus")]
        public async Task<IActionResult> GetOnStatus(string status)
        {
            //var list = await _identityService.GetNewStatusCases(status);
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
            return new BadRequestObjectResult("No Cases found.");
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

           
           
            if (list.Count() != 0)
            {

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
            return new BadRequestObjectResult("No Cases found.");
        }



        private bool CaseExists(int id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }
    }
}
