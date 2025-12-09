using System.Windows;
using GymManager.WPF.ViewModels.Licensing;

namespace GymManager.WPF.Views.Licensing;

/// <summary>
/// Ventana de activacion de licencia
/// </summary>
public partial class LicenseActivationView : Window
{
    public LicenseActivationView(LicenseActivationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    /// <summary>
    /// Indica si la licencia fue validada correctamente
    /// </summary>
    public bool IsLicenseValid => (DataContext as LicenseActivationViewModel)?.IsLicenseValid ?? false;
}