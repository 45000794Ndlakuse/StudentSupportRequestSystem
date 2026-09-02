using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;

namespace NotificationService.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly NotificationDbContext _context;

    public HealthController(NotificationDbContext context)
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
                    service = "NotificationService",
                    database = "Unavailable"
                });
            }

            return Ok(new
            {
                status = "Healthy",
                service = "NotificationService",
                database = "Connected"
            });
        }
        catch (Exception)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                service = "NotificationService",
                database = "Unavailable"
            });
        }
    }
}