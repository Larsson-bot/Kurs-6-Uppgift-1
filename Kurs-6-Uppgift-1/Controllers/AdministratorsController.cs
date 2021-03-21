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

        [HttpPost("create-customer")]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer model)
        {
            if (await _identityService.CreateCustomerAsync(model))
                return new OkObjectResult("Customer has been created");

            return new BadRequestObjectResult("Email already exists!");
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




        [AllowAnonymous]
        [HttpGet("customer/{id}")]
        public async Task<IActionResult> GetSpecificCustomer(int id)
        {
            if (_identityService.ValidateAccessRights(IdentityRequest()))
            {
                if (_context.Customers.Count() < id)
                {
                    return new BadRequestObjectResult("Customer does not exist");
                }
                else

                    return new OkObjectResult(await _identityService.GetSpecificCustomer(id));
            }
            return new UnauthorizedObjectResult("Unauthorized!");
      
        }


        private RequestUser IdentityRequest()
        {
            try
            {
                var user =  new RequestUser
                {
                    UserId = int.Parse(HttpContext.User.FindFirst("UserId").Value),
                    Token = Request.Headers["Authorization"].ToString().Split(" ")[1]
                };
                return user;
            }

            catch
            {
               CatchMessage();
            }
            return new RequestUser();

        }

        private IActionResult CatchMessage()
        {
            return new NotFoundObjectResult("Wrong User or Token.");
        }


    }
}
