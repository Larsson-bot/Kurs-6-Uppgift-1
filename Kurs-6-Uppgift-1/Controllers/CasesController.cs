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
        public async Task<ActionResult<IEnumerable<Case>>> GetCases()
        {
            return await _context.Cases.ToListAsync();
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

            var issue = _context.Cases.AsNoTracking().Where(x => x.Id == id).ToList();

            foreach (var errand in issue)
            {
                updatedCase.AdminstratorId = errand.AdminstratorId;
                updatedCase.Created = errand.Created;
                updatedCase.CustomerId = errand.CustomerId;
                updatedCase.LatestChange = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                updatedCase.Description = errand.Description;
            }

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
            if (await _identityService.CreateCaseAsync(model))
                return new OkResult();

            return new BadRequestResult();
        }

        // DELETE: api/Cases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCase(int id)
        {
            var @case = await _context.Cases.FindAsync(id);
            if (@case == null)
            {
                return NotFound();
            }

            _context.Cases.Remove(@case);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CaseExists(int id)
        {
            return _context.Cases.Any(e => e.Id == id);
        }
    }
}
