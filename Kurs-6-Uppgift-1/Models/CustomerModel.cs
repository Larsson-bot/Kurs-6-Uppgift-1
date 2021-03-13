using Kurs_6_Uppgift_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Models
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public virtual ICollection<Case> Cases { get; set; }
    }
}
