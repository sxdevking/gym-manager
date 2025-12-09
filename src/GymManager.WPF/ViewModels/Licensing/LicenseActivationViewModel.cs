using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymManager.Application.Common.Interfaces;

namespace GymManager.WPF.ViewModels.Licensing;

/// <summary>
/// ViewModel para la ventana de activacion de licencia
/// </summary>
public partial class LicenseActivationViewModel : ObservableObject
{
    private readonly ILicenseService _licenseService;
    private readonly IDongleService _dongleService;

    [ObservableProperty]
    private string _licenseKey = string.Empty;

    [ObservableProperty]
    private string _statusMessage = "Esperando dispositivo de seguridad...";

    [ObservableProperty]
    private string _dongleStatus = "No detectado";

    [ObservableProperty]
    private bool _isDongleConnected;

    [ObservableProperty]
    private bool _isActivating;

    [ObservableProperty]
    private bool _isLicenseValid;

    [ObservableProperty]
    private string _licenseInfo = string.Empty;

    public LicenseActivationViewModel(
        ILicenseService licenseService,
        IDongleService dongleService)
    {
        _licenseService = licenseService;
        _dongleService = dongleService;

        // Suscribirse a cambios de conexion del dongle
        _dongleService.DongleConnectionChanged += OnDongleConnectionChanged;

        // Verificar estado inicial
        CheckDongleStatus();
    }

    private void OnDongleConnectionChanged(object? sender, bool isConnected)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            IsDongleConnected = isConnected;
            DongleStatus = isConnected ? "Conectado" : "No detectado";
            StatusMessage = isConnected
                ? "Dispositivo detectado. Ingrese su clave de licencia."
                : "Conecte el dispositivo de seguridad USB.";
        });
    }

    [RelayCommand]
    private void CheckDongleStatus()
    {
        IsDongleConnected = _dongleService.IsDongleConnected();
        DongleStatus = IsDongleConnected ? "Conectado" : "No detectado";
        StatusMessage = IsDongleConnected
            ? "Dispositivo detectado. Ingrese su clave de licencia."
            : "Conecte el dispositivo de seguridad USB.";
    }

    [RelayCommand]
    private async Task ActivateLicenseAsync()
    {
        if (string.IsNullOrWhiteSpace(LicenseKey))
        {
            StatusMessage = "Ingrese una clave de licencia valida.";
            return;
        }

        if (!IsDongleConnected)
        {
            StatusMessage = "Conecte el dispositivo de seguridad primero.";
            return;
        }

        try
        {
            IsActivating = true;
            StatusMessage = "Activando licencia...";

            var result = await _licenseService.ActivateLicenseAsync(LicenseKey);

            if (result.IsValid)
            {
                IsLicenseValid = true;
                StatusMessage = result.Message;
                LicenseInfo = $"Tipo: {result.LicenseType}\n" +
                              $"Sucursales: {result.MaxBranches}\n" +
                              $"Usuarios: {result.MaxUsers}";
            }
            else
            {
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsActivating = false;
        }
    }

    [RelayCommand]
    private async Task ValidateExistingLicenseAsync()
    {
        try
        {
            IsActivating = true;
            StatusMessage = "Validando licencia...";

            var result = await _licenseService.ValidateLicenseAsync();

            if (result.IsValid)
            {
                IsLicenseValid = true;
                StatusMessage = "Licencia valida";

                var expiryText = result.DaysRemaining.HasValue
                    ? $"Expira en: {result.DaysRemaining} dias"
                    : "Licencia perpetua";

                LicenseInfo = $"Tipo: {result.LicenseType}\n" +
                              $"Sucursales: {result.MaxBranches}\n" +
                              $"Usuarios: {result.MaxUsers}\n" +
                              expiryText;
            }
            else
            {
                StatusMessage = result.Message;
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsActivating = false;
        }
    }
}