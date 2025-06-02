using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.Core.DTOs.User
{
    // UserCenter.Core/DTOs/UpdateUserDto.cs
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? NickName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public short? Gender { get; set; }
        public int? UserRole { get; set; }
    }

}
