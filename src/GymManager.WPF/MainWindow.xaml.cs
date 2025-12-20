using System.Windows;
using GymManager.WPF.ViewModels;
using GymManager.WPF.ViewModels.Classes;
using GymManager.WPF.ViewModels.Members;
using GymManager.WPF.ViewModels.Memberships;
using GymManager.WPF.ViewModels.Payments;
using GymManager.WPF.ViewModels.Reports;
using GymManager.WPF.ViewModels.Sales;
using GymManager.WPF.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace GymManager.WPF;

/// <summary>
/// Ventana principal de la aplicacion GymManager
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;

        ConfigureViewModels();

        // Cargar dashboard al iniciar
        Loaded += MainWindow_Loaded;
    }

    private void ConfigureViewModels()
    {
        // Members
        var membersViewModel = _serviceProvider.GetRequiredService<MembersViewModel>();
        MembersViewControl.DataContext = membersViewModel;

        // Memberships
        var membershipsViewModel = _serviceProvider.GetRequiredService<MembershipsViewModel>();
        MembershipsViewControl.DataContext = membershipsViewModel;

        // Payments
        var paymentsViewModel = _serviceProvider.GetRequiredService<PaymentsViewModel>();
        PaymentsViewControl.DataContext = paymentsViewModel;

        // Sales
        var salesViewModel = _serviceProvider.GetRequiredService<SalesViewModel>();
        SalesViewControl.DataContext = salesViewModel;

        // Classes
        var classesViewModel = _serviceProvider.GetRequiredService<ClassesViewModel>();
        ClassesViewControl.DataContext = classesViewModel;

        // Reports
        var reportsViewModel = _serviceProvider.GetRequiredService<ReportsViewModel>();
        ReportsViewControl.DataContext = reportsViewModel;

        // Settings
        var settingsViewModel = _serviceProvider.GetRequiredService<SettingsViewModel>();
        SettingsViewControl.DataContext = settingsViewModel;

        // Configurar carga de datos al mostrar cada vista
        _viewModel.PropertyChanged += async (s, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(MainViewModel.ShowMembers) when _viewModel.ShowMembers:
                    await membersViewModel.LoadMembersAsync();
                    break;
                case nameof(MainViewModel.ShowMemberships) when _viewModel.ShowMemberships:
                    await membershipsViewModel.InitializeAsync();
                    break;
                case nameof(MainViewModel.ShowPayments) when _viewModel.ShowPayments:
                    await paymentsViewModel.InitializeAsync();
                    break;
                case nameof(MainViewModel.ShowSales) when _viewModel.ShowSales:
                    await salesViewModel.InitializeAsync();
                    break;
                case nameof(MainViewModel.ShowClasses) when _viewModel.ShowClasses:
                    await classesViewModel.InitializeAsync();
                    break;
                case nameof(MainViewModel.ShowReports) when _viewModel.ShowReports:
                    await reportsViewModel.InitializeAsync();
                    break;
                case nameof(MainViewModel.ShowSettings) when _viewModel.ShowSettings:
                    await settingsViewModel.InitializeAsync();
                    break;
            }
        };
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDashboardAsync();
    }
}