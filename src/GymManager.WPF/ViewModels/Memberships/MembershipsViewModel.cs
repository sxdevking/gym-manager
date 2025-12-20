using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Domain.Enums;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace GymManager.WPF.ViewModels.Memberships;

/// <summary>
/// ViewModel para la vista principal de Membresías
/// </summary>
public partial class MembershipsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private ObservableCollection<MembershipListItem> _memberships = new();

    [ObservableProperty]
    private MembershipListItem? _selectedMembership;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private MembershipStatus? _filterStatus;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private int _totalCount;

    [ObservableProperty]
    private int _activeCount;

    [ObservableProperty]
    private int _expiringCount;

    [ObservableProperty]
    private int _expiredCount;

    public IEnumerable<MembershipStatus?> StatusFilters => new MembershipStatus?[]
    {
        null, // Todos
        MembershipStatus.ACTIVE,
        MembershipStatus.EXPIRED,
        MembershipStatus.FROZEN,
        MembershipStatus.CANCELLED
    };

    public MembershipsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await LoadMembershipsAsync();
        await LoadStatsAsync();
    }

    [RelayCommand]
    private async Task LoadMembershipsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var query = context.Memberships
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .Where(m => m.Member.DeletedAt == null)
                .AsQueryable();

            // Filtro por estado
            if (FilterStatus.HasValue)
            {
                query = query.Where(m => m.Status == FilterStatus.Value);
            }

            // Filtro por búsqueda
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var search = SearchText.ToLower();
                query = query.Where(m =>
                    m.Member.FirstName.ToLower().Contains(search) ||
                    m.Member.LastName.ToLower().Contains(search) ||
                    m.Member.MemberCode.ToLower().Contains(search) ||
                    m.Plan.Name.ToLower().Contains(search));
            }

            var now = DateTime.Now;

            var memberships = await query
                .OrderByDescending(m => m.StartDate)
                .Take(100)
                .Select(m => new MembershipListItem
                {
                    MembershipId = m.MembershipId,
                    MemberCode = m.Member.MemberCode,
                    MemberName = m.Member.FirstName + " " + m.Member.LastName,
                    PlanName = m.Plan.Name,
                    StartDate = m.StartDate,
                    EndDate = m.EndDate,
                    Status = m.Status,
                    PricePaid = m.PricePaid,
                    DaysRemaining = m.EndDate >= now
                        ? (int)(m.EndDate - now).TotalDays
                        : 0
                })
                .ToListAsync();

            Memberships.Clear();
            foreach (var membership in memberships)
            {
                Memberships.Add(membership);
            }

            TotalCount = Memberships.Count;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar membresías: {ex.Message}";
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
            var nextWeek = today.AddDays(7);

            ActiveCount = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.ACTIVE && m.EndDate >= today);

            ExpiringCount = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.ACTIVE &&
                                 m.EndDate >= today && m.EndDate <= nextWeek);

            ExpiredCount = await context.Memberships
                .CountAsync(m => m.Status == MembershipStatus.EXPIRED ||
                                 (m.Status == MembershipStatus.ACTIVE && m.EndDate < today));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando stats: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        await LoadMembershipsAsync();
    }

    [RelayCommand]
    private async Task FilterByStatusAsync(MembershipStatus? status)
    {
        FilterStatus = status;
        await LoadMembershipsAsync();
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        SearchText = string.Empty;
        FilterStatus = null;
        await LoadMembershipsAsync();
        await LoadStatsAsync();
    }

    partial void OnSearchTextChanged(string value)
    {
        // Auto-búsqueda después de escribir
        if (string.IsNullOrWhiteSpace(value) || value.Length >= 2)
        {
            _ = LoadMembershipsAsync();
        }
    }

    partial void OnFilterStatusChanged(MembershipStatus? value)
    {
        _ = LoadMembershipsAsync();
    }
}

/// <summary>
/// Item para la lista de membresías
/// </summary>
public class MembershipListItem
{
    public Guid MembershipId { get; set; }
    public string MemberCode { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipStatus Status { get; set; }
    public decimal PricePaid { get; set; }
    public int DaysRemaining { get; set; }

    public string StatusDisplay => Status switch
    {
        MembershipStatus.ACTIVE => "✅ Activa",
        MembershipStatus.EXPIRED => "⏰ Expirada",
        MembershipStatus.FROZEN => "❄️ Congelada",
        MembershipStatus.CANCELLED => "❌ Cancelada",
        _ => Status.ToString()
    };

    public string DaysRemainingDisplay => Status == MembershipStatus.ACTIVE
        ? DaysRemaining > 0 ? $"{DaysRemaining} días" : "Vence hoy"
        : "-";

    public string StartDateDisplay => StartDate.ToString("dd/MM/yyyy");
    public string EndDateDisplay => EndDate.ToString("dd/MM/yyyy");
}