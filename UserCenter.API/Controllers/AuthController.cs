using Microsoft.AspNetCore.Mvc;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Interfaces;

namespace UserCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IUserService _userService;

        public AuthController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            var result = await _authService.RegisterAsync(registerUserDto);
            if (!result.IsSuccess) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginUserDto)
        {
            var (result, tokenString) = await _authService.LoginAsync(loginUserDto);
            if (result.IsSuccess && result.Data != null)
            {
                // 设置 HttpOnly Cookie
                Response.Cookies.Append("token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 本地开发设为 false
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(result); // 可返回用户信息给前端使用
            }

            return BadRequest(result.Message ?? "Login failed");
        }

        // 登出让 Cookie 立即过期
        [HttpPost("signOut")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("token", "", new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // 立即过期
            });

            return Ok(new { message = "Logout success" });
        }
    }
}
