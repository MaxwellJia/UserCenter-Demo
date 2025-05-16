using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserCenter.Core.Configuration;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;
using UserCenter.Infrastructure.Constants;

namespace UserCenter.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Svae user information modified from the client
        /// </summary>
        /// <param name="saveChangesRequestDto"></param>
        /// <returns>SaveChangesRespondDto if succeeded</returns>
        public async Task<(SaveChangesRespondDto, string)> UpdateUserAsync(SaveChangesRequestDto saveChangesRequestDto)
        {
            var response = new SaveChangesRespondDto();

            var user = await _userManager.FindByIdAsync(saveChangesRequestDto.UserId);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = "User not found.";
                return (response, "");
            }

            user.NickName = saveChangesRequestDto.NickName;
            user.PhoneNumber = saveChangesRequestDto.Phone;
            user.Gender = (short)saveChangesRequestDto.Gender; // Change to short because of database type

            var result = await _userManager.UpdateAsync(user);

            String tokenString = "";
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Message = "User updated successfully.";
                response.Data = new SaveChangesRequestDto
                {
                    UserId = user.Id.ToString(),
                    NickName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    Avatar = user.AvatarUrl ?? Defaults.DefaultAvatar,
                    UserRole = user.UserRole ?? 0,
                    Phone = user.PhoneNumber ?? "",
                    Gender = user.Gender ?? 0,
                };
                tokenString = GenerateJwtToken(user);// Update token after user information changed
            }
            else
            {
                response.IsSuccess = false;
                response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
            }

            return (response, tokenString);
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
