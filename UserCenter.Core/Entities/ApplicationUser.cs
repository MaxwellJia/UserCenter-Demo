using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNetCore.Identity;


namespace UserCenter.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? NickName { get; set; }

    public string? AvatarUrl { get; set; }

    public short? Gender { get; set; }

    public int? UserStatus { get; set; }

    public DateTime? CreateTime { get; set; }

    public DateTime? UpdateTime { get; set; }

    public short? IsDelete { get; set; }

    public int? UserRole { get; set; }
}
