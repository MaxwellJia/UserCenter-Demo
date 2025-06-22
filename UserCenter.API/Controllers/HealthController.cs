using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserCenter.Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly UserCenterDbContext _context;

    public HealthController(UserCenterDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Ping()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");
            return Ok("Database is active");
        }
        catch (Exception ex)
        {
            return StatusCode(503, "Database not available: " + ex.Message);
        }
    }
}
