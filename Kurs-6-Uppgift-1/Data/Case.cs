using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Data
{
    public class Case
    {   
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; } 

        [Required]
        public int AdminstratorId { get; set; }

        [Column(TypeName ="nvarchar(max)")]
        public string Description { get; set; }

        [Required]
        public DateTime Created { get; set; }

        
        public DateTime LatestChange { get; set; }


        [Column(TypeName = "nvarchar(30)")]
        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
