using System.Windows;
using GymManager.WPF.ViewModels;

namespace GymManager.WPF;

/// <summary>
/// Ventana principal de la aplicacion GymManager
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Cargar dashboard al iniciar
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Cargar estadísticas del dashboard automáticamente
        await _viewModel.LoadDashboardAsync();
    }
}