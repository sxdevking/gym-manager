using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure.Persistence;
using GymManager.Infrastructure.Repositories;
using GymManager.Infrastructure.Services.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.Infrastructure;

/// <summary>
/// Configuración de inyección de dependencias para Infrastructure
/// Registra todos los servicios de infraestructura en el contenedor DI
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Agrega los servicios de infraestructura al contenedor de dependencias
    /// </summary>
    /// <param name="services">Colección de servicios</param>
    /// <param name="configuration">Configuración de la aplicación</param>
    /// <returns>Colección de servicios con Infrastructure registrado</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ═══════════════════════════════════════════════════════════
        // Base de datos - Entity Framework Core con PostgreSQL
        // ═══════════════════════════════════════════════════════════
        services.AddDbContext<GymDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(GymDbContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                }));

        // ═══════════════════════════════════════════════════════════
        // Repositorios y Unit of Work
        // ═══════════════════════════════════════════════════════════

        // Repositorio genérico - Scoped porque depende de DbContext
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Unit of Work - Scoped para manejar transacciones por request
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ═══════════════════════════════════════════════════════════
        // Servicios de Licenciamiento
        // ═══════════════════════════════════════════════════════════

        // EncryptionService: Singleton porque no tiene estado mutable
        services.AddSingleton<IEncryptionService, EncryptionService>();

        // DongleService: Singleton porque monitorea el hardware continuamente
        services.AddSingleton<IDongleService, DongleService>();

        // LicenseService: Scoped porque usa DbContext (que es Scoped)
        services.AddScoped<ILicenseService, LicenseService>();

        return services;
    }
}
