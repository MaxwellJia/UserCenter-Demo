using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Core.DTOs.User
{
    public class UserQueryDto
    {
        public required string UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public required string NickName { get; set; }
        public required string Avatar { get; set; }

        public required string Email { get; set; }
        public string Phone { get; set; } = string.Empty;

        public required int Gender { get; set; }
        public required int UserRole { get; set; }
    }
}
