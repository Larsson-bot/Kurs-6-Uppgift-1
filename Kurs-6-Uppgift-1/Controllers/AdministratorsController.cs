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
                return new OkResult();

            return new BadRequestResult();
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


   


        //[AllowAnonymous]
        [HttpGet("get-cases")]
        public async Task<IActionResult> GetCases()
        {

            return new OkObjectResult(await _identityService.GetCases());
        }

        [AllowAnonymous]
        [HttpPut("update-case{id}")]
        public async Task<IActionResult> UpdateCase(int id, [FromBody]Case updatedcase) 
        {
 
            //var date = new DateTime();
            //var issue = _context.Cases.AsNoTracking().Where(x => x.Id == id).ToList();

            //foreach (var errand in issue)
            //{
            //    updatedcase.Id = id;
            //    //updatedcase.AdminstratorId = errand.AdminstratorId;
            //    updatedcase.Created = errand.Created;
            //    //updatedcase.CustomerId = errand.CustomerId;
            //    updatedcase.LatestChange = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
            //    updatedcase.Description = errand.Description;
            //    updatedcase.Status = errand.Status;
            //}
     

    


            
            _context.Entry(updatedcase).State = EntityState.Modified;
    
            await _context.SaveChangesAsync();
            return new OkResult();
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
        [AllowAnonymous]
        [HttpGet("cases/getonstatus{status}")]
        public async Task<IActionResult> GetOnStatus(string status)
        {
            var list = await _identityService.GetNewStatusCases(status);
            if (list.Count() != 0)
            {
                return new OkObjectResult(list);
            }
            return new BadRequestObjectResult("No Cases found.");
        }

        private RequestUser IdentityRequest()
        {
            //HttpContext.Request.Headers.TryGetValue("UserId", out var _userId);
            //HttpContext.Request.Headers.TryGetValue("AccessToken", out var _accessToken);

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
