using JwtAuthenticationDemo.Api.Data;
using JwtAuthenticationDemo.Api.DTOs.Auth;
using JwtAuthenticationDemo.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public AuthController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existingUser = await _dbContext.Users
                            .FirstOrDefaultAsync(x => x.Email == request.Email);
        if (existingUser != null)
        {
            return Conflict("Email is already registered");
        }
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "User registered successfully",
            userId = user.Id
        });
    }
}