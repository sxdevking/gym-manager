using System.Windows;
using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure;
using GymManager.Infrastructure.Persistence;
using GymManager.WPF.ViewModels;
using GymManager.WPF.ViewModels.Licensing;
using GymManager.WPF.Views.Licensing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GymManager.WPF;

/// <summary>
/// Punto de entrada de la aplicacion WPF
/// </summary>
public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Registrar Infrastructure (incluye BD, repositorios y licenciamiento)
                services.AddInfrastructure(context.Configuration);

                // ═══════════════════════════════════════════════════════════
                // Registrar ViewModels
                // ═══════════════════════════════════════════════════════════
                services.AddTransient<MainViewModel>();
                services.AddTransient<LicenseActivationViewModel>();

                // ═══════════════════════════════════════════════════════════
                // Registrar Views
                // ═══════════════════════════════════════════════════════════
                services.AddTransient<MainWindow>();
                services.AddTransient<LicenseActivationView>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        try
        {
            // Verificar conexión a base de datos
            using var scope = _host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            if (!await context.Database.CanConnectAsync())
            {
                MessageBox.Show(
                    "No se pudo conectar a la base de datos.\nVerifique que PostgreSQL esté ejecutándose.",
                    "Error de Conexión",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown();
                return;
            }

            // TODO: Validar licencia aquí cuando esté listo el dongle
            // Por ahora, ir directo a MainWindow

            // Mostrar ventana principal
            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Error al iniciar la aplicación:\n\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
        }

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}