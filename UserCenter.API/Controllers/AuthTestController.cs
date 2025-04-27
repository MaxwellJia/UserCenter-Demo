using Microsoft.AspNetCore.Mvc;
using UserCenter.Core.Interfaces;

namespace UserCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthTestController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthTestController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _authService.RegisterAsync(username, password);
            if (user == null) return BadRequest("Register failed");
            return Ok(user);
        }
    }
}
