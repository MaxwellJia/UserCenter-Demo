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
using UserCenter.Infrastructure.Constants;
using UserCenter.Infrastructure.Helpers;
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
            var response = new RegisterRespondDto
            {
                IsSuccess = false,
            };

            // Custom Validation
            var validationMessage = ValidateRegisterLoginInput(registerUserDto, true);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                response.Message = validationMessage;
                return response;
            }

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

            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = TokenGenerator.GenerateToken(user, _jwtSettings);
            }
            else {
                response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return response;
        }

        /// <summary>
        /// Custom validation for register and login input
        /// 
        ///Username verification:
        ///Length: 3-32 characters.
        ///Limited to: letters, numbers, and underscores(a-zA-Z0-9_).
        ///
        ///Password verification:
        ///Length: at least 8 characters.
        ///Contains:
        ///One uppercase letter(A-Z).
        ///One lowercase letter(a-z).
        ///One number(0-9).
        ///One special character(widely supported special characters: !@#$%^&*()_+-=[]{}|;:'",.<>/?~`).
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        private string ValidateRegisterLoginInput(dynamic dto, bool isRegister = false)
        {
            // Username validation using regex
            if (string.IsNullOrWhiteSpace(dto.Username)) return "Username is required.";
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Username, "^(?=.{3,32}$)[a-zA-Z0-9_]+$"))
                return "Username must be 3-32 characters and can only contain letters, numbers, and underscores.";

            // Password validation using regex
            if (string.IsNullOrWhiteSpace(dto.Password)) return "Password is required.";
            if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{}|;:'"",.<>/?`~]).{8,}$"))
                return "Password must include uppercase, lowercase, number, and special character, and be at least 8 characters long.";

            // Email validation
            if (isRegister) {
                if (string.IsNullOrWhiteSpace(dto.Email)) return "Email is required.";
                if (!System.Text.RegularExpressions.Regex.IsMatch(dto.Email, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$"))
                    return "Please enter a valid email address.";
            }
            
            return string.Empty; // Valid input
        }

        /// <summary>
        /// Login a user and return LoginRespondDto
        /// </summary>
        /// <param name="loginRequestDto"></param>
        /// <returns>LoginRespondDto token if succeeded</returns>
        public async Task<(LoginRespondDto, string?)> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var response = new LoginRespondDto
            {
                IsSuccess = false,
            };

            // Custom Validation
            var validationMessage = ValidateRegisterLoginInput(loginRequestDto);
            if (!string.IsNullOrEmpty(validationMessage))
            {
                response.Message = validationMessage;
                return (response, "");
            }

            var user = await _userManager.FindByNameAsync(loginRequestDto.Username);

            if (user == null)
            {
                response.Message = "User not found";
                return (response, "");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);

            if (!result.Succeeded)
            {
                response.Message = "Invalid password";
                return (response, "");
            }

            response.IsSuccess = true;
            response.Data = new LoginUserDto
            {
                UserId = user.Id.ToString(),
                NickName = user.UserName ?? "",
                Email = user.Email ?? "",
                Avatar = user.AvatarUrl ?? Defaults.DefaultAvatar,
                UserRole = user.UserRole ?? 0,
                Phone = user.PhoneNumber ?? "",
                Gender = user.Gender ?? 0,
            };
            String Token = TokenGenerator.GenerateToken(user, _jwtSettings);

            return (response, Token);
        }
    }
}
