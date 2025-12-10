using System.Windows;
using GymManager.Domain.Entities;
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
    private MemberFormViewModel? _memberFormViewModel;

    public MainWindow(MainViewModel viewModel, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _serviceProvider = serviceProvider;
        DataContext = _viewModel;

        // Configurar ViewModels
        SetupMembersViewModel();
        SetupMemberFormViewModel();

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

    /// <summary>
    /// Configura el ViewModel de la lista de miembros
    /// </summary>
    private void SetupMembersViewModel()
    {
        _membersViewModel = _serviceProvider.GetRequiredService<MembersViewModel>();
        MembersViewControl.DataContext = _membersViewModel;

        // Suscribirse al evento para abrir el formulario
        _membersViewModel.OnEditMemberRequested += OnEditMemberRequested;
    }

    /// <summary>
    /// Configura el ViewModel del formulario de miembro
    /// </summary>
    private void SetupMemberFormViewModel()
    {
        _memberFormViewModel = _serviceProvider.GetRequiredService<MemberFormViewModel>();
        MemberFormViewControl.DataContext = _memberFormViewModel;

        // Suscribirse a eventos del formulario
        _memberFormViewModel.OnSaveCompleted += OnMemberSaveCompleted;
        _memberFormViewModel.OnCancelRequested += OnMemberFormCancelled;
    }

    /// <summary>
    /// Maneja la solicitud de editar/crear miembro
    /// </summary>
    private async void OnEditMemberRequested(Member? member)
    {
        if (_memberFormViewModel == null) return;

        // Navegar al formulario
        _viewModel.ShowMembers = false;
        _viewModel.ShowMemberForm = true;
        _viewModel.Title = member == null ? "GymManager - Nuevo Miembro" : "GymManager - Editar Miembro";

        // Inicializar el formulario
        if (member == null)
        {
            await _memberFormViewModel.InitializeNewAsync();
        }
        else
        {
            await _memberFormViewModel.InitializeEditAsync(member.MemberId);
        }
    }

    /// <summary>
    /// Maneja cuando se guarda exitosamente el miembro
    /// </summary>
    private async void OnMemberSaveCompleted()
    {
        // Regresar a la lista de miembros
        _viewModel.ShowMemberForm = false;
        _viewModel.ShowMembers = true;
        _viewModel.Title = "GymManager - Miembros";

        // Refrescar la lista
        if (_membersViewModel != null)
        {
            await _membersViewModel.RefreshAfterSaveAsync();
        }
    }

    /// <summary>
    /// Maneja cuando se cancela el formulario
    /// </summary>
    private void OnMemberFormCancelled()
    {
        // Regresar a la lista de miembros sin refrescar
        _viewModel.ShowMemberForm = false;
        _viewModel.ShowMembers = true;
        _viewModel.Title = "GymManager - Miembros";
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadDashboardAsync();
    }
}
