using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GymManager.WPF.ViewModels.Sales;

/// <summary>
/// ViewModel para la vista principal de Ventas
/// </summary>
public partial class SalesViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<SaleListItem> _sales = new();

    [ObservableProperty]
    private SaleListItem? _selectedSale;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private DateTime? _filterDateFrom;

    [ObservableProperty]
    private DateTime? _filterDateTo;

    [ObservableProperty]
    private SaleStatus? _filterStatus;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Stats
    [ObservableProperty]
    private decimal _todayTotal;

    [ObservableProperty]
    private int _todayCount;

    [ObservableProperty]
    private decimal _weekTotal;

    [ObservableProperty]
    private decimal _monthTotal;

    public IEnumerable<SaleStatus?> StatusFilters => new SaleStatus?[]
    {
        null,
        SaleStatus.COMPLETED,
        SaleStatus.PENDING,
        SaleStatus.CANCELLED,
        SaleStatus.REFUNDED
    };

    public SalesViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        FilterDateFrom = DateTime.Now.AddDays(-30);
        FilterDateTo = DateTime.Now;
    }

    public async Task InitializeAsync()
    {
        await LoadSalesAsync();
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task LoadSalesAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.Sales
                .Include(s => s.Member)
                .Include(s => s.SaleItems)
                .AsQueryable();

            // Filtro por fechas
            if (FilterDateFrom.HasValue)
            {
                query = query.Where(s => s.SaleDate >= FilterDateFrom.Value);
            }
            if (FilterDateTo.HasValue)
            {
                var endDate = FilterDateTo.Value.AddDays(1);
                query = query.Where(s => s.SaleDate < endDate);
            }

            // Filtro por estado
            if (FilterStatus.HasValue)
            {
                query = query.Where(s => s.Status == FilterStatus.Value);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(s =>
                    s.TicketNumber.ToLower().Contains(search) ||
                    (s.Member != null && (
                        s.Member.FirstName.ToLower().Contains(search) ||
                        s.Member.LastName.ToLower().Contains(search) ||
                        s.Member.MemberCode.ToLower().Contains(search))));
            }

            var sales = await query
                .OrderByDescending(s => s.SaleDate)
                .Take(100)
                .Select(s => new SaleListItem
                {
                    SaleId = s.SaleId,
                    TicketNumber = s.TicketNumber,
                    MemberCode = s.Member != null ? s.Member.MemberCode : "-",
                    MemberName = s.Member != null ? s.Member.FirstName + " " + s.Member.LastName : "Venta directa",
                    SaleDate = s.SaleDate,
                    ItemsCount = s.SaleItems.Count(),
                    Subtotal = s.Subtotal,
                    DiscountAmount = s.DiscountAmount,
                    TotalAmount = s.Total,
                    Status = s.Status
                })
                .ToListAsync();

            Sales.Clear();
            foreach (var sale in sales)
            {
                Sales.Add(sale);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar ventas: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadStatsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var today = DateTime.Today;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            // Ventas de hoy
            var todaySales = await context.Sales
                .Where(s => s.SaleDate >= today && s.Status == SaleStatus.COMPLETED)
                .ToListAsync();

            TodayTotal = todaySales.Sum(s => s.Total);
            TodayCount = todaySales.Count;

            // Ventas de la semana
            WeekTotal = await context.Sales
                .Where(s => s.SaleDate >= weekStart && s.Status == SaleStatus.COMPLETED)
                .SumAsync(s => s.Total);

            // Ventas del mes
            MonthTotal = await context.Sales
                .Where(s => s.SaleDate >= monthStart && s.Status == SaleStatus.COMPLETED)
                .SumAsync(s => s.Total);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando stats: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task NewSaleAsync()
    {
        // TODO: Abrir ventana de nueva venta (POS)
    }

    [RelayCommand]
    private async Task ViewSaleDetailsAsync()
    {
        if (SelectedSale == null) return;
        // TODO: Abrir detalle de venta
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        FilterDateFrom = DateTime.Now.AddDays(-30);
        FilterDateTo = DateTime.Now;
        await LoadSalesAsync();
        await LoadStatsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length >= 2)
        {
            _ = LoadSalesAsync();
        }
    }
}

/// <summary>
/// Item para la lista de ventas
/// </summary>
public class SaleListItem
{
    public Guid SaleId { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string MemberCode { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int ItemsCount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public SaleStatus Status { get; set; }

    public string StatusDisplay => Status switch
    {
        SaleStatus.COMPLETED => "✅ Completada",
        SaleStatus.PENDING => "⏳ Pendiente",
        SaleStatus.CANCELLED => "❌ Cancelada",
        SaleStatus.REFUNDED => "↩️ Reembolsada",
        _ => Status.ToString()
    };

    public string TotalDisplay => $"${TotalAmount:N2}";
    public string DateDisplay => SaleDate.ToString("dd/MM/yyyy HH:mm");
    public string ItemsDisplay => $"{ItemsCount} producto(s)";
}