using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure.Persistence;
using GymManager.Infrastructure.Repositories;
using GymManager.Infrastructure.Services.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.Infrastructure;

/// <summary>
/// Configuracion de inyeccion de dependencias para Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Base de datos con conversion automatica a snake_case
        services.AddDbContext<GymDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(GymDbContext).Assembly.FullName))
            .UseSnakeCaseNamingConvention()); // <-- IMPORTANTE: Convierte PascalCase a snake_case

        // Repositorios
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Servicios de licenciamiento
        services.AddSingleton<IEncryptionService, EncryptionService>();
        services.AddSingleton<IDongleService, DongleService>();
        services.AddScoped<ILicenseService, LicenseService>();

        return services;
    }
}

