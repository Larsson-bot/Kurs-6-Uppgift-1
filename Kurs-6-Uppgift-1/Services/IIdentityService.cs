using Kurs_6_Uppgift_1.Data;
using Kurs_6_Uppgift_1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Services
{
    public interface IIdentityService
    {
        Task<bool> CreateUserAsync(SignUpModel model);

        Task<bool> CreateCustomerAsync(Customer model);

        Task<SignInResponseModel> SignInAsync(string email, string password);

        Task<bool> CreateCaseAsync(Case model);

        Task<IEnumerable<CaseModel>> GetCases();

        Task<CustomerModel> GetSpecificCustomer(int id);

        Task<IEnumerable<Case>> GetNewStatusCases(string status);
    }
}
