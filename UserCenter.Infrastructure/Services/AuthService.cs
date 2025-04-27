using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;

namespace UserCenter.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser?> RegisterAsync(string username, string password)
        {
            var user = new ApplicationUser
            {
                UserName = username,
                CreateTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow,
                IsDelete = 0,
                UserStatus = 1
            };

            var result = await _userManager.CreateAsync(user, password);

            return result.Succeeded ? user : null;
        }

    }
}
