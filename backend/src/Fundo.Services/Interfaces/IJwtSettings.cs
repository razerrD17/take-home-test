namespace Fundo.Services.Interfaces;

public interface IJwtSettings
{
    string Secret { get; }
    string Issuer { get; }
    string Audience { get; }
    int ExpirationInMinutes { get; }
}
