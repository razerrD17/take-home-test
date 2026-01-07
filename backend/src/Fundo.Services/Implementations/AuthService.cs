using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fundo.Domain.Interfaces;
using Fundo.Services.Common;
using Fundo.Services.DTOs;
using Fundo.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Fundo.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IJwtSettings jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings;
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        if (user == null)
        {
            return ServiceResult<LoginResponse>.BadRequest("Invalid username or password");
        }

        if (!user.IsActive)
        {
            return ServiceResult<LoginResponse>.BadRequest("User account is disabled");
        }

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            return ServiceResult<LoginResponse>.BadRequest("Invalid username or password");
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user.Username, user.Role);

        return ServiceResult<LoginResponse>.Ok(new LoginResponse
        {
            Token = token,
            ExpiresIn = _jwtSettings.ExpirationInMinutes * 60
        });
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private string GenerateJwtToken(string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
