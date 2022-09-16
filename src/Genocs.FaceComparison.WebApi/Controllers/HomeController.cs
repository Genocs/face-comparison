using Microsoft.AspNetCore.Mvc;

namespace Genocs.FaceComparison.WebApi.Controllers;

[Route("")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Genocs Face Comparison Service");
    }

    [HttpGet("ping")]
    public IActionResult GetPing()
    {
        return Ok("pong");
    }
}
