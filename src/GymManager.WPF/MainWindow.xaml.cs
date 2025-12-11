using System.Windows;
using GymManager.WPF.ViewModels;
using GymManager.WPF.ViewModels.Members;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.WPF;

/// <summary>
/// Ventana principal de la aplicacion GymManager
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;
    private MembersViewModel? _membersViewModel;

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;

        // Configurar el DataContext de MembersView
        _membersViewModel = _serviceProvider.GetRequiredService<MembersViewModel>();
        MembersViewControl.DataContext = _membersViewModel;

        // Cargar miembros cuando se muestre la vista
        _viewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ShowMembers) && _viewModel.ShowMembers)
            {
                if (_membersViewModel != null)
                {
                    await _membersViewModel.LoadMembersAsync();
                }
            }
        };

        // Cargar dashboard al iniciar
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDashboardAsync();
    }

    // ═══════════════════════════════════════════════════════════
    // NAVEGACION DEL MENU (si se usan eventos Click en XAML)
    // ═══════════════════════════════════════════════════════════

    private void OnDashboardClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Dashboard");
    }

    private void OnMembersClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Members");
    }

    private void OnMembershipsClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Memberships");
    }

    private void OnPaymentsClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Payments");
    }

    private void OnClassesClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Classes");
    }

    private void OnSalesClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Sales");
    }

    private void OnReportsClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Reports");
    }

    private void OnSettingsClick(object sender, RoutedEventArgs e)
    {
        NavigateTo("Settings");
    }

    private async void NavigateTo(string view)
    {
        // Ocultar todas las vistas
        _viewModel.ShowDashboard = false;
        _viewModel.ShowMembers = false;
        _viewModel.ShowMemberForm = false;

        switch (view)
        {
            case "Dashboard":
                _viewModel.ShowDashboard = true;
                _viewModel.Title = "GymManager - Dashboard";
                await _viewModel.LoadDashboardAsync();
                break;

            case "Members":
                _viewModel.ShowMembers = true;
                _viewModel.Title = "GymManager - Miembros";
                break;

            case "Memberships":
                _viewModel.Title = "GymManager - Membresias";
                MessageBox.Show("Modulo de Membresias en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            case "Payments":
                _viewModel.Title = "GymManager - Pagos";
                MessageBox.Show("Modulo de Pagos en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            case "Classes":
                _viewModel.Title = "GymManager - Clases";
                MessageBox.Show("Modulo de Clases en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            case "Sales":
                _viewModel.Title = "GymManager - Ventas";
                MessageBox.Show("Modulo de Ventas en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            case "Reports":
                _viewModel.Title = "GymManager - Reportes";
                MessageBox.Show("Modulo de Reportes en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            case "Settings":
                _viewModel.Title = "GymManager - Configuracion";
                MessageBox.Show("Modulo de Configuracion en desarrollo", "Proximamente",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                _viewModel.ShowDashboard = true;
                break;

            default:
                _viewModel.ShowDashboard = true;
                _viewModel.Title = "GymManager - Dashboard";
                break;
        }
    }
}