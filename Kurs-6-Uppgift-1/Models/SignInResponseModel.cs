using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Models
{
    public class SignInResponseModel
    {
        public bool Succeeded { get; set; }
        public SignInResult Result { get; set; }
    }
    public class SignInResult
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
