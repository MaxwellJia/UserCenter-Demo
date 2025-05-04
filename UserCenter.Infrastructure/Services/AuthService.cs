using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Configuration;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using UserCenter.API.DTOs.Auth;

namespace UserCenter.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registerUserDto"></param>
        /// <returns>User if succeeded</returns>
        public async Task<RegisterRespondDto> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser
            {
                UserName = registerUserDto.Username,
                CreateTime = DateTime.UtcNow,
                UpdateTime = DateTime.UtcNow,
                IsDelete = 0,
                UserStatus = 1,
                UserRole = 0,
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);

            var response = new RegisterRespondDto
            {
                IsSuccess = false,
            };

            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = GenerateJwtToken(user);
            }
            else {
                response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return response;
        }

        /// <summary>
        /// Login a user and return JWT token
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns>JWT token if succeeded</returns>
        public async Task<LoginRespondDto> LoginAsync(LoginUserDto loginUserDto)
        {
            var response = new LoginRespondDto
            {
                IsSuccess = false,
            };

            var user = await _userManager.FindByNameAsync(loginUserDto.Username);

            if (user == null)
            {
                response.Message = "User not found";
                return response;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false);

            if (!result.Succeeded)
            {
                response.Message = "Invalid password";
                return response;
            }

            response.IsSuccess = true;
            response.Data = GenerateJwtToken(user);
            return response;
        }

        /// <summary>
        /// Generate JWT token by user information
        /// </summary>
        /// <param name="user">user information</param>
        /// <returns>JWT token string</returns>
        private string GenerateJwtToken(ApplicationUser user)

        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
