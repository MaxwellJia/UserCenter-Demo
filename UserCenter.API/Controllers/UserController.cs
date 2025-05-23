using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User didn't log in");
            }

            var isAdmin = await _userService.IsUserAdminAsync(userId);
            if (!isAdmin)
            {

                return StatusCode(StatusCodes.Status403Forbidden, "Insufficient permissions, only administrators can access");
            }

            var users = await _userService.GetAllUsersAsync();
            return Ok(new { data = users, total = users.Count });
        }

    }
}
