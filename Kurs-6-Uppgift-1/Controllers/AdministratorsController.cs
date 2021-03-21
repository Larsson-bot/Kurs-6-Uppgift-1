using Kurs_6_Uppgift_1.Data;
using Kurs_6_Uppgift_1.Models;
using Kurs_6_Uppgift_1.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdministratorsController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly SqlDbContext _context;

        public AdministratorsController(IIdentityService identityService, SqlDbContext context)
        {
            _identityService = identityService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            if (_context.Users.Any())
            {
                return new UnauthorizedObjectResult("There can only be one Administrator");
            }
            if (await _identityService.CreateUserAsync(model))
            {
                return new OkResult();
            }
            return new BadRequestResult();
        }

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInModel model)
        {
            var response = await _identityService.SignInAsync(model.Email, model.Password);
            if (response.Succeeded)
                return new OkObjectResult(response.Result);

            return new BadRequestObjectResult("Invalid Email or Password");
        }

        [HttpGet("GetAdminInfo")]
        public async Task<IActionResult> GetAdminAsync()
        {
            var admin = await _context.Users.FirstOrDefaultAsync();
            if (admin != null)
                return new OkObjectResult(admin);

            return new BadRequestObjectResult("Admin does not exist!");
        }
    }
}
