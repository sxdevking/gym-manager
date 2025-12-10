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

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;

        // Configurar el DataContext de MembersView
        var membersViewModel = _serviceProvider.GetRequiredService<MembersViewModel>();
        MembersViewControl.DataContext = membersViewModel;

        // Cargar miembros cuando se muestre la vista
        _viewModel.PropertyChanged += async (s, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ShowMembers) && _viewModel.ShowMembers)
            {
                await membersViewModel.LoadMembersAsync();
            }
        };

        // Cargar dashboard al iniciar
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDashboardAsync();
    }
}