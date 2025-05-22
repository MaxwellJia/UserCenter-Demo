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
        private readonly ICookieService _cookieService;

        public UserController(IUserService userService, ICookieService cookieService)
        {
            _userService = userService;
            _cookieService = cookieService;
        }

        

        [HttpPost("saveChanges")]
        public async Task<IActionResult> SaveChanges([FromBody] SaveChangesRequestDto dto)
        {
            var principal = _cookieService.ValidateToken(Request);
            if (principal == null)
            {
                return Unauthorized(new { message = "Invalid or expired token" });
            }

            var (result, tokenString) = await _userService.UpdateUserAsync(dto);
            if (result.IsSuccess && result.Data != null)
            {
                _cookieService.AppendAuthTokenCookie(Response, tokenString);
                return Ok(result);
            }

            return BadRequest(result.Message ?? "Change failed");
        }

    }
}
