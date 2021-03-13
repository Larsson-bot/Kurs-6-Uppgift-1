using Kurs_6_Uppgift_1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Models
{
    public class CaseModel
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int AdminstratorId { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }


        public DateTime LatestChange { get; set; }

        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
