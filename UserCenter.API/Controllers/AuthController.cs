using Microsoft.AspNetCore.Mvc;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Interfaces;
using UserCenter.Infrastructure.Services;

namespace UserCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        private readonly IUserService _userService;

        // 这里可以使用 CookieService 来处理 Cookie
        private readonly ICookieService _cookieService;

        public AuthController(IAuthService authService, IUserService userService, ICookieService cookieService)
        {
            _authService = authService;
            _userService = userService;
            _cookieService = cookieService;
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
                _cookieService.AppendAuthTokenCookie(Response, tokenString);



                return Ok(result); // 可返回用户信息给前端使用
            }

            return BadRequest(result.Message ?? "Login failed");
        }

        // 登出让 Cookie 立即过期
        [HttpPost("signOut")]
        public IActionResult Logout()
        {
            _cookieService.ExpireAuthTokenCookie(Response);

            return Ok(new { message = "Logout success" });
        }
    }
}
