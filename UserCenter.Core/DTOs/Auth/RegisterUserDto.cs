using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Core.DTOs.Auth
{
    public class RegisterUserDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(32)]
        [RegularExpression("^[a-zA-Z0-9_]+$")]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z\\d]).{8,64}$")]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")]
        public string Email { get; set; }
        //public string? NickName { get; set; }

        //public string? AvatarUrl { get; set; }

        //public int Gender { get; set; }

        //public int UserStatus { get; set; }

        //public int IsDelete { get; set; }

        //public int UserRole { get; set; }

        //public string? Email { get; set; }

        //public string? Phone { get; set; }
    }
}
