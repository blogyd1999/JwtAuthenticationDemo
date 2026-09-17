using JwtAuthenticationDemo.Api.Entities;
namespace  JwtAuthenticationDemo.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user, out DateTime expiresAt);
}