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
        // var claims = User.Claims.Select(c => new
        // {
        //     c.Type,
        //     c.Value
        // });
        // return Ok(claims);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("admin")]
    public IActionResult AdminOnly()
    {
        return Ok("Only admin can access this endpoint");
    }
}