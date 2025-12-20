using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Enums;
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

    // ═══════════════════════════════════════════════════════════
    // VISIBILIDAD DE VISTAS
    // ═══════════════════════════════════════════════════════════

    [ObservableProperty]
    private bool _showDashboard = true;

    [ObservableProperty]
    private bool _showMembers;

    [ObservableProperty]
    private bool _showMemberships;

    [ObservableProperty]
    private bool _showPayments;

    [ObservableProperty]
    private bool _showSales;

    [ObservableProperty]
    private bool _showClasses;

    [ObservableProperty]
    private bool _showReports;

    [ObservableProperty]
    private bool _showSettings;

    public MainViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Oculta todas las vistas
    /// </summary>
    private void HideAllViews()
    {
        ShowDashboard = false;
        ShowMembers = false;
        ShowMemberships = false;
        ShowPayments = false;
        ShowSales = false;
        ShowClasses = false;
        ShowReports = false;
        ShowSettings = false;
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

            TotalMembers = await context.Members.CountAsync(m => m.IsActive && m.DeletedAt == null);

            ActiveMembers = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.ACTIVE && m.IsActive);

            TodayAttendance = await context.Attendances
                .CountAsync(a => a.CheckInTime.Date == DateTime.UtcNow.Date);

            var nextWeek = DateTime.UtcNow.AddDays(7);
            ExpiringMemberships = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.ACTIVE
                              && m.EndDate <= nextWeek
                              && m.EndDate >= DateTime.UtcNow);

            var todayDayOfWeek = (int)DateTime.UtcNow.DayOfWeek;
            ScheduledClasses = await context.ClassSchedules
                .CountAsync(cs => cs.DayOfWeek == todayDayOfWeek && cs.IsAvailable);

            var firstDayOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            MonthlyRevenue = await context.Payments
                .Where(p => p.PaymentDate >= firstDayOfMonth
                         && p.Status == PaymentStatus.COMPLETED)
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
    private async Task NavigateToDashboardAsync()
    {
        HideAllViews();
        ShowDashboard = true;
        CurrentView = "Dashboard";
        Title = "GymManager - Dashboard";
        await LoadDashboardAsync();
    }

    [RelayCommand]
    private void NavigateToMembers()
    {
        HideAllViews();
        ShowMembers = true;
        CurrentView = "Members";
        Title = "GymManager - Miembros";
    }

    [RelayCommand]
    private void NavigateToMemberships()
    {
        HideAllViews();
        ShowMemberships = true;
        CurrentView = "Memberships";
        Title = "GymManager - Membresías";
    }

    [RelayCommand]
    private void NavigateToPayments()
    {
        HideAllViews();
        ShowPayments = true;
        CurrentView = "Payments";
        Title = "GymManager - Pagos";
    }

    [RelayCommand]
    private void NavigateToSales()
    {
        HideAllViews();
        ShowSales = true;
        CurrentView = "Sales";
        Title = "GymManager - Ventas";
    }

    [RelayCommand]
    private void NavigateToClasses()
    {
        HideAllViews();
        ShowClasses = true;
        CurrentView = "Classes";
        Title = "GymManager - Clases";
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        HideAllViews();
        ShowReports = true;
        CurrentView = "Reports";
        Title = "GymManager - Reportes";
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        HideAllViews();
        ShowSettings = true;
        CurrentView = "Settings";
        Title = "GymManager - Configuración";
    }
}
