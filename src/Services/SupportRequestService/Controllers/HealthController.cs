using Microsoft.AspNetCore.Mvc;

namespace SupportRequestService.Controllers
{
    [ApiController]
    [Route("health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                service = "SupportRequestService"
            });
        }
    }
}