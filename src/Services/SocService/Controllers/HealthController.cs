using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocService.Data;

namespace SocService.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly SocDbContext _context;

    public HealthController(SocDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            // Check if the database is reachable
            var databaseHealthy = await _context.Database.CanConnectAsync();

            if (!databaseHealthy)
            {
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    service = "SocService",
                    database = "Unavailable"
                });
            }

            return Ok(new
            {
                status = "Healthy",
                service = "SocService",
                database = "Connected"
            });
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                service = "SocService",
                database = "Unavailable"
            });
        }
    }
}