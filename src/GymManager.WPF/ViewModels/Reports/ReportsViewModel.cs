using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GymManager.WPF.ViewModels.Reports;

/// <summary>
/// ViewModel para la vista de Reportes
/// </summary>
public partial class ReportsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private DateTime _dateFrom = DateTime.Now.AddMonths(-1);

    [ObservableProperty]
    private DateTime _dateTo = DateTime.Now;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Datos del resumen
    [ObservableProperty]
    private decimal _totalRevenue;

    [ObservableProperty]
    private decimal _membershipRevenue;

    [ObservableProperty]
    private decimal _salesRevenue;

    [ObservableProperty]
    private int _newMembersCount;

    [ObservableProperty]
    private int _activeMembershipsCount;

    [ObservableProperty]
    private int _totalAttendances;

    [ObservableProperty]
    private int _classesHeld;

    [ObservableProperty]
    private double _avgClassOccupancy;

    // Datos para gráficos
    [ObservableProperty]
    private ObservableCollection<RevenueByMonth> _monthlyRevenue = new();

    [ObservableProperty]
    private ObservableCollection<TopProduct> _topProducts = new();

    [ObservableProperty]
    private ObservableCollection<MembershipByPlan> _membershipsByPlan = new();

    public ReportsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await LoadReportDataAsync();
    }

    [RelayCommand]
    private async Task LoadReportDataAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            // Ingresos totales
            var payments = await context.Payments
                .Where(p => p.PaymentDate >= DateFrom &&
                            p.PaymentDate <= DateTo.AddDays(1) &&
                            p.Status == PaymentStatus.COMPLETED)
                .ToListAsync();

            TotalRevenue = payments.Sum(p => p.Amount);
            MembershipRevenue = payments.Where(p => p.MembershipId != Guid.Empty).Sum(p => p.Amount);
            // SalesRevenue se calcula de otra forma ya que Payment no tiene SaleId
            SalesRevenue = TotalRevenue - MembershipRevenue;

            // Nuevos miembros
            NewMembersCount = await context.Members
                .CountAsync(m => m.CreatedAt >= DateFrom &&
                                 m.CreatedAt <= DateTo.AddDays(1) &&
                                 m.DeletedAt == null);

            // Membresías activas
            var today = DateTime.Today;
            ActiveMembershipsCount = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.ACTIVE &&
                                 m.EndDate >= today);

            // Asistencias
            TotalAttendances = await context.Attendances
                .CountAsync(a => a.CheckInTime >= DateFrom &&
                                 a.CheckInTime <= DateTo.AddDays(1));

            // Ingresos por mes
            await LoadMonthlyRevenueAsync(context);

            // Top productos
            await LoadTopProductsAsync(context);

            // Membresías por plan
            await LoadMembershipsByPlanAsync(context);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error cargando reportes: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadMonthlyRevenueAsync(GymDbContext context)
    {
        var monthlyData = await context.Payments
            .Where(p => p.PaymentDate >= DateFrom.AddMonths(-6) &&
                        p.Status == PaymentStatus.COMPLETED)
            .GroupBy(p => new { p.PaymentDate.Year, p.PaymentDate.Month })
            .Select(g => new RevenueByMonth
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Revenue = g.Sum(p => p.Amount)
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToListAsync();

        MonthlyRevenue.Clear();
        foreach (var item in monthlyData)
        {
            MonthlyRevenue.Add(item);
        }
    }

    private async Task LoadTopProductsAsync(GymDbContext context)
    {
        var topProds = await context.SaleItems
            .Include(si => si.Product)
            .Where(si => si.Sale.SaleDate >= DateFrom &&
                         si.Sale.SaleDate <= DateTo.AddDays(1) &&
                         si.Sale.Status == SaleStatus.COMPLETED)
            .GroupBy(si => new { si.ProductId, si.Product.Name })
            .Select(g => new TopProduct
            {
                ProductName = g.Key.Name,
                QuantitySold = g.Sum(si => si.Quantity),
                TotalRevenue = g.Sum(si => si.Subtotal)
            })
            .OrderByDescending(p => p.QuantitySold)
            .Take(10)
            .ToListAsync();

        TopProducts.Clear();
        foreach (var item in topProds)
        {
            TopProducts.Add(item);
        }
    }

    private async Task LoadMembershipsByPlanAsync(GymDbContext context)
    {
        var today = DateTime.Today;
        var byPlan = await context.Memberships
            .Include(m => m.Plan)
            .Where(m => m.Status == MembershipStatus.ACTIVE &&
                        m.EndDate >= today)
            .GroupBy(m => m.Plan.Name)
            .Select(g => new MembershipByPlan
            {
                PlanName = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(p => p.Count)
            .ToListAsync();

        MembershipsByPlan.Clear();
        foreach (var item in byPlan)
        {
            MembershipsByPlan.Add(item);
        }
    }

    [RelayCommand]
    private async Task ExportToPdfAsync()
    {
        // TODO: Implementar exportación a PDF con QuestPDF
    }

    [RelayCommand]
    private async Task ExportToExcelAsync()
    {
        // TODO: Implementar exportación a Excel
    }

    [RelayCommand]
    private void SetDateRange(string range)
    {
        var today = DateTime.Today;
        switch (range)
        {
            case "today":
                DateFrom = today;
                DateTo = today;
                break;
            case "week":
                DateFrom = today.AddDays(-(int)today.DayOfWeek);
                DateTo = today;
                break;
            case "month":
                DateFrom = new DateTime(today.Year, today.Month, 1);
                DateTo = today;
                break;
            case "quarter":
                var quarterStart = new DateTime(today.Year, ((today.Month - 1) / 3) * 3 + 1, 1);
                DateFrom = quarterStart;
                DateTo = today;
                break;
            case "year":
                DateFrom = new DateTime(today.Year, 1, 1);
                DateTo = today;
                break;
        }
        _ = LoadReportDataAsync();
    }
}

public class RevenueByMonth
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMM yyyy");
}

public class TopProduct
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class MembershipByPlan
{
    public string PlanName { get; set; } = string.Empty;
    public int Count { get; set; }
}