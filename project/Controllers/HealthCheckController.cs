using Microsoft.AspNetCore.Mvc;

namespace project.Controllers;

[ApiController]
[Route("/api/health")]
public class HealthCheckController : ControllerBase
{

    [HttpGet]
    public IActionResult Health()
    {
        return Ok("Server Healthy");
    }
    
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("Pong");
    }
    
    
    [HttpGet("hello")]
    public IActionResult Hello()
    {
        return Ok("World!");
    }
}