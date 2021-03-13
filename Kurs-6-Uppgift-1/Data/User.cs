using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kurs_6_Uppgift_1.Data
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [Column(TypeName ="nvarchar(50)")]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string LastName { get; set; }


        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required]
        public byte[] UserHash { get; set; }

        [Required]
        public byte[] UserSalt { get; set; }

        public void CreatePasswordWithHash(string password)
        {
            using(var hmac = new HMACSHA512())
            {
                UserSalt = hmac.Key;
                UserHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public bool ValidatePasswordHash(string password)
        {
            using(var hmac = new HMACSHA512(UserSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for(int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != UserHash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}
