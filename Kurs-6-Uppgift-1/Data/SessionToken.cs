using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Data
{
    public class SessionToken
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varbinary(max)")]
        public string AccessToken { get; set; }


        [Required]
        [Column(TypeName ="DateTime")]
        public DateTime ExpireDate { get; set; }
    }
}
