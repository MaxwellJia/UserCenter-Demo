using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserCenter.Core.DTOs.Auth;
using UserCenter.Core.DTOs.User;
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
        public async Task<IActionResult> GetAllUsers([FromQuery] FilterUserDto filter)
        {
            foreach (var key in Request.Query.Keys)
            {
                Console.WriteLine($"[QUERY PARAM] {key}: {Request.Query[key]}");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User didn't log in");

            if (!await _userService.IsUserAdminAsync(userId))
                return StatusCode(StatusCodes.Status403Forbidden, "Insufficient permissions, only administrators can access");

            var (users, total) = await _userService.FilterUsersAsync(filter);
            return Ok(new { data = users, total });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest("Invalid user ID format");
            }

            var success = await _userService.SoftDeleteUserAsync(guid);
            if (!success)
            {
                return NotFound("User not found or already deleted");
            }

            return Ok("User deleted successfully");
        }



        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            if (!Guid.TryParse(id, out var guid))
                return BadRequest("Invalid user ID format");

            if (guid != dto.Id)
                return BadRequest("ID mismatch");

            var success = await _userService.UpdateUserAsync(dto);
            if (!success)
                return NotFound("User not found or already deleted");

            return Ok("User updated successfully");
        }

    }
}
