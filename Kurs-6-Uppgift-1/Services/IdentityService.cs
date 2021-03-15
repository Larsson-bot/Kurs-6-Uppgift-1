using Kurs_6_Uppgift_1.Data;
using Kurs_6_Uppgift_1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SqlDbContext _context;
        private IConfiguration Configuration { get; }
        public IdentityService(SqlDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }



        public async Task<bool> CreateUserAsync(SignUpModel model)
        {
            if(!_context.Users.Any(u => u.Email == model.Email))
            {
                try
                {
                    var user = new User()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    };
                    user.CreatePasswordWithHash(model.Password);
                     _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch
                {
                   
                }
            }
            return false;
        }

        public async Task<bool> CreateCustomerAsync(Customer model)
        {
            if(!_context.Customers.Any(c => c.Email == model.Email))
            {
                try
                {
                    var customer = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch
                {

                }
   
            }
            return false;
        }

        public async Task<SignInResponseModel> SignInAsync(string email, string password)
        {
            try
            {
                
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                
                var token = _context.Tokens.FirstOrDefault(t => t.UserId == user.Id);
                if (token != null)
                {
                    if (DateTime.Now > token.ExpireDate)
                    {
                        _context.Tokens.Remove(token);
                        _context.SaveChanges();
                        token = null;
                    }
                }
              
                
                if(token != null)
                {
                    return new SignInResponseModel
                    {
                        Succeeded = true,
                        Result = new SignInResult
                        {
                            Id = user.Id,
                            Email = user.Email,
                            AccessToken = token.AccessToken,
                            ExpireDate = token.ExpireDate
                        }
                    };
                }

                if(user != null)
                {
                    try
                    {
                        if (user.ValidatePasswordHash(password))
                        {
                            var tokenHandler = new JwtSecurityTokenHandler();

                            
                            var expiresDate = DateTime.Now.AddMinutes(5);

                            var _secretKey = Encoding.UTF8.GetBytes(Configuration.GetSection("SecretKey").Value);
                            var tokenDescriptor = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(new Claim[]
                                { new Claim("UserId", user.Id.ToString()), new Claim("Expires", expiresDate.ToString()) }),
                                Expires = expiresDate,
                                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKey), SecurityAlgorithms.HmacSha512Signature)
                            };
                            var _accessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
                            _context.Tokens.Add(new SessionToken { UserId = user.Id, AccessToken = _accessToken, ExpireDate = expiresDate });
                            await _context.SaveChangesAsync();
                            return new SignInResponseModel
                            {
                                Succeeded = true,
                                Result = new SignInResult
                                {
                                    Id = user.Id,
                                    Email = user.Email,
                                    AccessToken = _accessToken,
                                    ExpireDate = expiresDate
                                }
                            };          
                        }
                    }
                    catch 
                    { 
                  
                    }
                }
            }
            catch
            {

            }
            return new SignInResponseModel
            {
                Succeeded = false
            };
        }

        public async Task<bool> CreateCaseAsync(Case model)
        {
            var date = DateTime.Now;

        

            try
            {
                var errand = new Case
                {
                    AdminstratorId = 1,
                    CustomerId = model.CustomerId,
                    Created = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second),
                    LatestChange = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second),
                    Status = "Not Started",
                    Description = model.Description

                };

                try
                {
                    _context.Cases.Add(errand);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch
                {

                }
            }
            catch
            {
            
            }
            return false;
        }

        public async Task<IEnumerable<CaseModel>> GetCases()
        {
            var cases = new List<CaseModel>();

            foreach(var errand in await _context.Cases.Include(c => c.Customer).ToListAsync())
            {
                var customer = _context.Customers.FirstOrDefault(c => c.Id == errand.CustomerId);
                cases.Add(new CaseModel
                {
                    Id = errand.Id,
                    AdminstratorId = errand.AdminstratorId,
                    CustomerId = errand.CustomerId,
                    Created = errand.Created,
                    LatestChange = errand.LatestChange,
                    Description = errand.Description,
                    Status = errand.Status,
                    Customer = customer
                    
                });
            }
            return cases;
        }

        public async Task<CustomerModel> GetSpecificCustomer(int id)
        {
            
            
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            var cases =  _context.Cases.Where(x => x.CustomerId == customer.Id).ToList();

            var customerwithcases = new CustomerModel()
            {
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Id = customer.Id,
                Email = customer.Email,
                Cases = cases
            };

            return customerwithcases;
        }

        public bool ValidateAccessRights(RequestUser requestUser)
        {
            if (_context.Tokens.Any(u => u.AccessToken == requestUser.Token && u.UserId == requestUser.UserId))
                 return true;

            return false;


        }

        public async Task<IEnumerable<Case>> GetNewStatusCases(string status)
        {
            var list = await _context.Cases.Include(x => x.Customer).Where(x => x.Status == status).ToListAsync();

            foreach(var @case in list)
            {
                var customer =  _context.Customers.FirstOrDefault(c => c.Id == @case.CustomerId);

                @case.Customer = customer;
            }
            _context.SaveChanges();
            return list;
        }
    }
    }

