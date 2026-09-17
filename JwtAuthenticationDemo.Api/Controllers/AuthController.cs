using JwtAuthenticationDemo.Api.Data;
using JwtAuthenticationDemo.Api.DTOs.Auth;
using JwtAuthenticationDemo.Api.Entities;
using JwtAuthenticationDemo.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtService _jwtService;
    public AuthController(AppDbContext dbContext, IJwtService jwtService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
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
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
        if (!isPasswordValid) return Unauthorized("Invalid email or password.");
        var token = _jwtService.GenerateToken(user, out var expiresAt);
        return Ok(new LoginResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt
        });
    }
}