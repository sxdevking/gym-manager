using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Domain.Entities;
using GymManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace GymManager.WPF.ViewModels.Settings;

/// <summary>
/// ViewModel para la vista de Configuración
/// </summary>
public partial class SettingsViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private string _successMessage = string.Empty;

    // Información de Sucursal
    [ObservableProperty]
    private Guid _branchId;

    [ObservableProperty]
    private string _branchName = string.Empty;

    [ObservableProperty]
    private string _branchCode = string.Empty;

    [ObservableProperty]
    private string _branchAddress = string.Empty;

    [ObservableProperty]
    private string _branchPhone = string.Empty;

    [ObservableProperty]
    private string _branchEmail = string.Empty;

    // Personalización
    [ObservableProperty]
    private string _primaryColor = "#3B82F6";

    [ObservableProperty]
    private string _secondaryColor = "#1E293B";

    [ObservableProperty]
    private string _logoPath = string.Empty;

    [ObservableProperty]
    private string _businessName = string.Empty;

    [ObservableProperty]
    private string _taxId = string.Empty;

    [ObservableProperty]
    private string _receiptFooter = string.Empty;

    // Planes de membresía
    [ObservableProperty]
    private ObservableCollection<MembershipPlanItem> _membershipPlans = new();

    [ObservableProperty]
    private MembershipPlanItem? _selectedPlan;

    // Métodos de pago
    [ObservableProperty]
    private ObservableCollection<PaymentMethodItem> _paymentMethods = new();

    [ObservableProperty]
    private PaymentMethodItem? _selectedPaymentMethod;

    // Categorías de productos
    [ObservableProperty]
    private ObservableCollection<ProductCategoryItem> _productCategories = new();

    // Información de Licencia
    [ObservableProperty]
    private string _licenseKey = string.Empty;

    [ObservableProperty]
    private string _licenseType = string.Empty;

    [ObservableProperty]
    private DateTime? _licenseExpiration;

    [ObservableProperty]
    private int _maxBranches;

    [ObservableProperty]
    private int _maxUsers;

    public SettingsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await LoadBranchSettingsAsync();
        await LoadMembershipPlansAsync();
        await LoadPaymentMethodsAsync();
        await LoadProductCategoriesAsync();
        await LoadLicenseInfoAsync();
    }

    private async Task LoadBranchSettingsAsync()
    {
        try
        {
            IsLoading = true;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var branch = await context.Branches
                .Include(b => b.Settings)
                .Where(b => b.IsActive && b.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (branch != null)
            {
                BranchId = branch.BranchId;
                BranchName = branch.BranchName;
                BranchCode = branch.BranchCode;
                BranchAddress = branch.Address ?? string.Empty;
                BranchPhone = branch.Phone ?? string.Empty;
                BranchEmail = branch.Email ?? string.Empty;

                if (branch.Settings != null)
                {
                    PrimaryColor = branch.Settings.PrimaryColor ?? "#3B82F6";
                    SecondaryColor = branch.Settings.SecondaryColor ?? "#1E293B";
                    LogoPath = branch.Settings.LogoPath ?? string.Empty;
                    BusinessName = branch.Settings.BusinessName ?? string.Empty;
                    TaxId = branch.Settings.TaxId ?? string.Empty;
                    ReceiptFooter = branch.Settings.ReceiptFooter ?? string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error cargando configuración: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadMembershipPlansAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var plans = await context.MembershipPlans
                .Where(p => p.DeletedAt == null)
                .OrderBy(p => p.Price)
                .Select(p => new MembershipPlanItem
                {
                    PlanId = p.PlanId,
                    PlanName = p.Name,
                    DurationDays = p.DurationDays,
                    Price = p.Price,
                    IsActive = p.IsAvailable
                })
                .ToListAsync();

            MembershipPlans.Clear();
            foreach (var plan in plans)
            {
                MembershipPlans.Add(plan);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando planes: {ex.Message}");
        }
    }

    private async Task LoadPaymentMethodsAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var methods = await context.PaymentMethods
                .OrderBy(pm => pm.Name)
                .Select(pm => new PaymentMethodItem
                {
                    PaymentMethodId = pm.PaymentMethodId,
                    MethodName = pm.Name,
                    RequiresReference = pm.RequiresReference
                })
                .ToListAsync();

            PaymentMethods.Clear();
            foreach (var method in methods)
            {
                PaymentMethods.Add(method);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando métodos de pago: {ex.Message}");
        }
    }

    private async Task LoadProductCategoriesAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var categories = await context.ProductCategories
                .Where(pc => pc.DeletedAt == null)
                .OrderBy(pc => pc.Name)
                .Select(pc => new ProductCategoryItem
                {
                    CategoryId = pc.CategoryId,
                    CategoryName = pc.Name,
                    IsActive = pc.IsActive
                })
                .ToListAsync();

            ProductCategories.Clear();
            foreach (var category in categories)
            {
                ProductCategories.Add(category);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando categorías: {ex.Message}");
        }
    }

    private async Task LoadLicenseInfoAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var license = await context.Licenses
                .Where(l => l.IsActive)
                .FirstOrDefaultAsync();

            if (license != null)
            {
                LicenseKey = MaskLicenseKey(license.LicenseKey);
                LicenseType = license.LicenseType.ToString();
                LicenseExpiration = license.ExpiresAt;
                MaxBranches = license.MaxBranches;
                MaxUsers = license.MaxUsers;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error cargando licencia: {ex.Message}");
        }
    }

    private string MaskLicenseKey(string key)
    {
        if (string.IsNullOrEmpty(key) || key.Length < 8)
            return "****-****-****-****";

        return $"{key.Substring(0, 4)}-****-****-{key.Substring(key.Length - 4)}";
    }

    [RelayCommand]
    private async Task SaveBranchSettingsAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<GymDbContext>();

            var branch = await context.Branches
                .Include(b => b.Settings)
                .FirstOrDefaultAsync(b => b.BranchId == BranchId);

            if (branch != null)
            {
                branch.BranchName = BranchName;
                branch.Address = BranchAddress;
                branch.Phone = BranchPhone;
                branch.Email = BranchEmail;
                branch.UpdatedAt = DateTime.UtcNow;

                if (branch.Settings != null)
                {
                    branch.Settings.PrimaryColor = PrimaryColor;
                    branch.Settings.SecondaryColor = SecondaryColor;
                    branch.Settings.LogoPath = LogoPath;
                    branch.Settings.BusinessName = BusinessName;
                    branch.Settings.TaxId = TaxId;
                    branch.Settings.ReceiptFooter = ReceiptFooter;
                    branch.Settings.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
                SuccessMessage = "Configuración guardada correctamente";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error guardando: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void SelectLogo()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Title = "Seleccionar Logo",
            Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp|Todos|*.*"
        };

        if (dialog.ShowDialog() == true)
        {
            LogoPath = dialog.FileName;
        }
    }

    [RelayCommand]
    private async Task RefreshAsync()
    {
        await InitializeAsync();
    }
}

public class MembershipPlanItem
{
    public Guid PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    public string PriceDisplay => $"${Price:N2}";
    public string DurationDisplay => DurationDays switch
    {
        1 => "1 día",
        7 => "1 semana",
        30 or 31 => "1 mes",
        90 => "3 meses",
        180 => "6 meses",
        365 => "1 año",
        _ => $"{DurationDays} días"
    };
    public string StatusDisplay => IsActive ? "✅ Activo" : "❌ Inactivo";
}

public class PaymentMethodItem
{
    public Guid PaymentMethodId { get; set; }
    public string MethodName { get; set; } = string.Empty;
    public bool RequiresReference { get; set; }
    public string StatusDisplay => RequiresReference ? "📝 Requiere ref." : "✅";
}

public class ProductCategoryItem
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string StatusDisplay => IsActive ? "✅" : "❌";
}