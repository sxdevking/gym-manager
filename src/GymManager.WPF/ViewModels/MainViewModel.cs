using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Application.Common.Interfaces;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.WPF.ViewModels;

/// <summary>
/// ViewModel principal de la aplicacion
/// Maneja la navegacion y el estado general
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private string _currentView = "Dashboard";

    [ObservableProperty]
    private string _title = "GymManager - Dashboard";

    [ObservableProperty]
    private string _userName = "Administrador";

    [ObservableProperty]
    private string _branchName = "Sucursal Principal";

    // ═══════════════════════════════════════════════════════════
    // ESTADÍSTICAS DEL DASHBOARD
    // ═══════════════════════════════════════════════════════════

    [ObservableProperty]
    private int _totalMembers;

    [ObservableProperty]
    private int _activeMembers;

    [ObservableProperty]
    private int _todayAttendance;

    [ObservableProperty]
    private decimal _monthlyRevenue;

    [ObservableProperty]
    private int _expiringMemberships;

    [ObservableProperty]
    private int _scheduledClasses;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = "Listo";

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Carga las estadisticas del dashboard
    /// </summary>
    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Cargando estadisticas...";

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            // Cargar estadísticas
            TotalMembers = await context.Members.CountAsync(m => m.IsActive && m.DeletedAt == null);

            ActiveMembers = await context.Memberships
                .CountAsync(m => m.Status == Domain.Enums.MembershipStatus.Active && m.IsActive);

            TodayAttendance = await context.Attendances
                .CountAsync(a => a.CheckInTime.Date == DateTime.UtcNow.Date);

            // Membresías por vencer en los próximos 7 días
            var nextWeek = DateTime.UtcNow.AddDays(7);
            ExpiringMemberships = await context.Memberships
                .CountAsync(m => m.Status == Domain.Enums.MembershipStatus.Active
                              && m.EndDate <= nextWeek
                              && m.EndDate >= DateTime.UtcNow);

            // Clases programadas para hoy
            var todayDayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
            ScheduledClasses = await context.ClassSchedules
                .CountAsync(cs => cs.DayOfWeek == todayDayOfWeek && cs.IsAvailable && cs.IsActive);

            // Ingresos del mes actual
            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            MonthlyRevenue = await context.Payments
                .Where(p => p.PaymentDate >= firstDayOfMonth
                         && p.Status == Domain.Enums.PaymentStatus.Completed)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            StatusMessage = $"Actualizado: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ═══════════════════════════════════════════════════════════
    // COMANDOS DE NAVEGACIÓN
    // ═══════════════════════════════════════════════════════════

    [RelayCommand]
    private void NavigateToDashboard()
    {
        CurrentView = "Dashboard";
        Title = "GymManager - Dashboard";
    }

    [RelayCommand]
    private void NavigateToMembers()
    {
        CurrentView = "Members";
        Title = "GymManager - Miembros";
    }

    [RelayCommand]
    private void NavigateToMemberships()
    {
        CurrentView = "Memberships";
        Title = "GymManager - Membresías";
    }

    [RelayCommand]
    private void NavigateToPayments()
    {
        CurrentView = "Payments";
        Title = "GymManager - Pagos";
    }

    [RelayCommand]
    private void NavigateToSales()
    {
        CurrentView = "Sales";
        Title = "GymManager - Ventas";
    }

    [RelayCommand]
    private void NavigateToClasses()
    {
        CurrentView = "Classes";
        Title = "GymManager - Clases";
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        CurrentView = "Reports";
        Title = "GymManager - Reportes";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = "Settings";
        Title = "GymManager - Configuración";
    }
}