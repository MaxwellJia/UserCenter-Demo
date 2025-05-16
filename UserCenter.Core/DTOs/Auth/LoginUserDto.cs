using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Core.DTOs.Auth
{
    public class LoginUserDto
    {
        public required string UserId { get; set; }
        public required string Avatar { get; set; } = default!;
        public required string NickName { get; set; } = default!;
        public required string Email { get; set; } = default!;

        public required int UserRole { get; set; }
        public required int Gender { get; set; }

        public required string Phone { get; set; } = default!;
    }
}
