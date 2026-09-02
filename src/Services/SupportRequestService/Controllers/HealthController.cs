using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupportRequestService.Data;

namespace SupportRequestService.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    private readonly SupportRequestDbContext _context;

    public HealthController(SupportRequestDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            await _context.Database.ExecuteSqlRawAsync("SELECT 1");

            return Ok(new
            {
                status = "Healthy",
                service = "SupportRequestService",
                database = "Connected"
            });
        }
        catch
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                service = "SupportRequestService",
                database = "Disconnected"
            });
        }
    }
}