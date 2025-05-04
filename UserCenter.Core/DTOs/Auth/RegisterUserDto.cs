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
        [StringLength(50, MinimumLength = 6)]
        public required string Username { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public required string Password { get; set; }

        public required string Email { get; set; }
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
