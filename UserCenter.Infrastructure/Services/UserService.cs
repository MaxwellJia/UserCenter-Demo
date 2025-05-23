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
using UserCenter.Core.DTOs.User;
using UserCenter.Core.Entities;
using UserCenter.Core.Interfaces;
using UserCenter.Infrastructure.Constants;
using UserCenter.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;

namespace UserCenter.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        public UserService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// 获取系统中所有用户的信息（仅供管理员使用）
        /// </summary>
        /// <returns>包含所有用户基本信息的列表（UserQueryDto）</returns>
        public async Task<List<UserQueryDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync(); ;

            // 可以根据实际需要做投影映射
            return users.Select(user => new UserQueryDto
            {
                UserId = user.Id.ToString(),
                NickName = user.NickName ?? "",
                Email = user.Email ?? "",
                Avatar = user.AvatarUrl ?? Defaults.DefaultAvatar,
                Gender = user.Gender ?? 0,
                UserRole = user.UserRole ?? 0,
                UserName = user.UserName ?? "",
                Password = user.PasswordHash ?? "",
                Phone = user.PhoneNumber ?? "",
            }).ToList();
        }

        /// <summary>
        /// 检查指定用户是否为管理员角色（UserRole == "1"）
        /// </summary>
        /// <param name="userId">用户的唯一标识符（Id）</param>
        /// <returns>如果用户存在且角色为管理员，返回 true；否则返回 false</returns>
        public async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && user.UserRole == 1;
        }

        /// <summary>
        /// Save user information modified from the client
        /// </summary>
        /// <param name="saveChangesRequestDto"></param>
        /// <returns>Tuple of SaveChangesRespondDto and new JWT token string if succeeded</returns>
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

            // Update Nickname and Phone
            user.NickName = saveChangesRequestDto.NickName;
            user.PhoneNumber = saveChangesRequestDto.Phone;
            user.Gender = (short)saveChangesRequestDto.Gender; // assuming your DB column is smallint
            user.AvatarUrl = saveChangesRequestDto.Avatar;
            
            // Update Email using UserManager
            if (user.Email != saveChangesRequestDto.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, saveChangesRequestDto.Email);
                if (!setEmailResult.Succeeded)
                {
                    // 打印错误信息到控制台（或使用 ILogger 记录）
                     foreach (var error in setEmailResult.Errors)
                    {
                        Console.WriteLine($"[SetEmailAsync Error] Code: {error.Code}, Description: {error.Description}");
                    }
                    response.IsSuccess = false;
                    response.Message = string.Join("; ", setEmailResult.Errors.Select(e => e.Description));
                    return (response, "");
                }
            }

            // Apply changes
            var result = await _userManager.UpdateAsync(user);

            string tokenString = "";
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Message = "User updated successfully.";
                response.Data = new SaveChangesRequestDto
                {
                    UserId = user.Id.ToString(),
                    NickName = user.NickName ?? "",
                    Email = user.Email ?? "",
                    Avatar = user.AvatarUrl ?? Defaults.DefaultAvatar,
                    UserRole = user.UserRole ?? 0,
                    Phone = user.PhoneNumber ?? "",
                    Gender = user.Gender ?? 0,
                };

                // Regenerate JWT token with updated info
                tokenString = TokenGenerate.GenerateToken(user, _jwtSettings);
            }
            else
            {
                response.IsSuccess = false;
                response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
            }

            return (response, tokenString);
        }
    }
}
