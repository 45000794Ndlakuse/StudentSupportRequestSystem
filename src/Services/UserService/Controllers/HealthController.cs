using Microsoft.AspNetCore.Mvc;
using UserService.Data;

namespace UserService.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly UserDbContext _context;

    public HealthController(UserDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var databaseHealthy =
                await _context.Database.CanConnectAsync();

            if (!databaseHealthy)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    service = "UserService",
                    database = "Unavailable"
                });
            }

            return Ok(new
            {
                status = "Healthy",
                service = "UserService",
                database = "Connected"
            });
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                service = "UserService",
                database = "Unavailable"
            });
        }
    }
}