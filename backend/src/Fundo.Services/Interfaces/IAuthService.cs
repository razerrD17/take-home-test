using Fundo.Services.Common;
using Fundo.Services.DTOs;

namespace Fundo.Services.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
