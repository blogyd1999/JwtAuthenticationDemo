using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthenticationDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [Authorize] //Only an authenticated user can access this endpoint.
    [HttpGet("profile")]
    public IActionResult GetProfile()
    {
        return Ok(new
        {
            message = "You are authenitcated!",
            user = User.Identity?.Name,
            email = User.FindFirst("email")?.Value
        });
    }
}