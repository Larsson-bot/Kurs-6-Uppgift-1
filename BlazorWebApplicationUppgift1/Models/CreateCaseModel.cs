
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebApplicationUppgift1.Models
{
    public class CreateCaseModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int AdminstratorId { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }


        public DateTime LatestChange { get; set; }

        public string Status { get; set; }
        
        public virtual CustomerModel Customer { get; set; }

    }
}
