using Fundo.Services.Implementations;
using Fundo.Services.Interfaces;
using Fundo.Services.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Fundo.Services.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IJwtSettings jwtSettings)
    {
        services.AddSingleton(jwtSettings);
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<IAuthService, AuthService>();

        // Register AutoMapper
        services.AddAutoMapper(typeof(LoanMappingProfile).Assembly);

        return services;
    }
}
