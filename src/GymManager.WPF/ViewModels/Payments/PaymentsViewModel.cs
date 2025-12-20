using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GymManager.WPF.ViewModels.Payments;

/// <summary>
/// ViewModel para la vista principal de Pagos
/// </summary>
public partial class PaymentsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<PaymentListItem> _payments = new();

    [ObservableProperty]
    private PaymentListItem? _selectedPayment;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private DateTime? _filterDateFrom;

    [ObservableProperty]
    private DateTime? _filterDateTo;

    [ObservableProperty]
    private PaymentStatus? _filterStatus;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    // Stats
    [ObservableProperty]
    private decimal _todayTotal;

    [ObservableProperty]
    private decimal _weekTotal;

    [ObservableProperty]
    private decimal _monthTotal;

    [ObservableProperty]
    private int _todayCount;

    public IEnumerable<PaymentStatus?> StatusFilters => new PaymentStatus?[]
    {
        null,
        PaymentStatus.COMPLETED,
        PaymentStatus.PENDING,
        PaymentStatus.REFUNDED,
        PaymentStatus.FAILED
    };

    public PaymentsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        FilterDateFrom = DateTime.Now.AddDays(-30);
        FilterDateTo = DateTime.Now;
    }

    public async Task InitializeAsync()
    {
        await LoadPaymentsAsync();
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task LoadPaymentsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.Payments
                .Include(p => p.Member)
                .Include(p => p.PaymentMethod)
                .AsQueryable();

            // Filtro por fechas
            if (FilterDateFrom.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= FilterDateFrom.Value);
            }
            if (FilterDateTo.HasValue)
            {
                var endDate = FilterDateTo.Value.AddDays(1);
                query = query.Where(p => p.PaymentDate < endDate);
            }

            // Filtro por estado
            if (FilterStatus.HasValue)
            {
                query = query.Where(p => p.Status == FilterStatus.Value);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(p =>
                    (p.Member != null && (
                        p.Member.FirstName.ToLower().Contains(search) ||
                        p.Member.LastName.ToLower().Contains(search) ||
                        p.Member.MemberCode.ToLower().Contains(search))) ||
                    (p.Reference != null && p.Reference.ToLower().Contains(search)));
            }

            var payments = await query
                .OrderByDescending(p => p.PaymentDate)
                .Take(100)
                .Select(p => new PaymentListItem
                {
                    PaymentId = p.PaymentId,
                    MemberCode = p.Member != null ? p.Member.MemberCode : "-",
                    MemberName = p.Member != null ? p.Member.FirstName + " " + p.Member.LastName : "Sin miembro",
                    PaymentMethodName = p.PaymentMethod.Name,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    Status = p.Status,
                    ReferenceNumber = p.Reference ?? "-",
                    Concept = p.MembershipId != Guid.Empty ? "Membresía" : "Otro"
                })
                .ToListAsync();

            Payments.Clear();
            foreach (var payment in payments)
            {
                Payments.Add(payment);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar pagos: {ex.Message}";
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

            // Pagos de hoy
            var todayPayments = await context.Payments
                .Where(p => p.PaymentDate >= today && p.Status == PaymentStatus.COMPLETED)
                .ToListAsync();

            TodayTotal = todayPayments.Sum(p => p.Amount);
            TodayCount = todayPayments.Count;

            // Pagos de la semana
            WeekTotal = await context.Payments
                .Where(p => p.PaymentDate >= weekStart && p.Status == PaymentStatus.COMPLETED)
                .SumAsync(p => p.Amount);

            // Pagos del mes
            MonthTotal = await context.Payments
                .Where(p => p.PaymentDate >= monthStart && p.Status == PaymentStatus.COMPLETED)
                .SumAsync(p => p.Amount);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando stats: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadPaymentsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        FilterDateFrom = DateTime.Now.AddDays(-30);
        FilterDateTo = DateTime.Now;
        await LoadPaymentsAsync();
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task FilterTodayAsync()
    {
        FilterDateFrom = DateTime.Today;
        FilterDateTo = DateTime.Today;
        await LoadPaymentsAsync();
    }

    [RelayCommand]
    private async Task FilterWeekAsync()
    {
        var today = DateTime.Today;
        FilterDateFrom = today.AddDays(-(int)today.DayOfWeek);
        FilterDateTo = today;
        await LoadPaymentsAsync();
    }

    [RelayCommand]
    private async Task FilterMonthAsync()
    {
        var today = DateTime.Today;
        FilterDateFrom = new DateTime(today.Year, today.Month, 1);
        FilterDateTo = today;
        await LoadPaymentsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length >= 2)
        {
            _ = LoadPaymentsAsync();
        }
    }
}

/// <summary>
/// Item para la lista de pagos
/// </summary>
public class PaymentListItem
{
    public Guid PaymentId { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string PaymentMethodName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Concept { get; set; } = string.Empty;

    public string StatusDisplay => Status switch
    {
        PaymentStatus.COMPLETED => "✅ Completado",
        PaymentStatus.PENDING => "⏳ Pendiente",
        PaymentStatus.REFUNDED => "↩️ Reembolsado",
        PaymentStatus.FAILED => "❌ Fallido",
        _ => Status.ToString()
    };

    public string AmountDisplay => $"${Amount:N2}";
    public string DateDisplay => PaymentDate.ToString("dd/MM/yyyy HH:mm");
}