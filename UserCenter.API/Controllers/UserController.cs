using Microsoft.AspNetCore.Mvc;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.Interfaces;

namespace UserCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("saveChanges")]
        public async Task<IActionResult> SaveChanges([FromBody] SaveChangesRequestDto saveChangesRequestDto)
        {
            var (result, tokenString) = await _userService.UpdateUserAsync(saveChangesRequestDto);
            if (result.IsSuccess && result.Data != null)
            {
                Response.Cookies.Append("token", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // 本地开发设为 false
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });

                return Ok(result);
            }

            return BadRequest(result.Message ?? "Change failed");
        }
    }
}
