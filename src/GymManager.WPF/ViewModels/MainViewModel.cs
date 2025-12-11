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
    // CONTENIDO DINÁMICO
    // ═══════════════════════════════════════════════════════════

    [ObservableProperty]
    private object? _currentContent;

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
    private bool _showMemberForm;

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
        ShowMemberForm = false;
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
                .CountAsync(cs => cs.DayOfWeek == todayDayOfWeek && cs.IsAvailable && cs.IsActive);

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
        CurrentView = "Memberships";
        Title = "GymManager - Membresías";
        MessageBox.Show("Módulo de Membresías - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void NavigateToPayments()
    {
        CurrentView = "Payments";
        Title = "GymManager - Pagos";
        MessageBox.Show("Módulo de Pagos - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void NavigateToSales()
    {
        CurrentView = "Sales";
        Title = "GymManager - Ventas";
        MessageBox.Show("Módulo de Ventas - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void NavigateToClasses()
    {
        CurrentView = "Classes";
        Title = "GymManager - Clases";
        MessageBox.Show("Módulo de Clases - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void NavigateToReports()
    {
        CurrentView = "Reports";
        Title = "GymManager - Reportes";
        MessageBox.Show("Módulo de Reportes - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        CurrentView = "Settings";
        Title = "GymManager - Configuración";
        MessageBox.Show("Módulo de Configuración - Próximamente", "En Desarrollo",
            MessageBoxButton.OK, MessageBoxImage.Information);
    }
}