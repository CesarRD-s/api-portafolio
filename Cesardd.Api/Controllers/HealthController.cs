using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Cesardd.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableRateLimiting("HealthPolicy")]
    public class HealthController(IConfiguration configuration) : ControllerBase
    {
        private static readonly string[] RequiredConfiguration =
        [
            "Security:ApiKey",
            "Resend:ApiKey",
            "Resend:FromEmail",
            "Resend:ContactEmail"
        ];

        [HttpGet]
        public IActionResult Get()
        {
            Response.Headers["Cache-Control"] = "no-store";

            var isReady = RequiredConfiguration.All(
                key => !string.IsNullOrWhiteSpace(configuration[key])
            );

            return isReady
                ? Ok(new { status = "ready" })
                : StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new { status = "not_ready" }
                );
        }
    }
}
