using GymManager.WPF.ViewModels.Members;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace GymManager.WPF.Views.Members;

/// <summary>
/// Ventana modal para agregar/editar miembros
/// </summary>
public partial class MemberFormWindow : Window
{
    private readonly MemberFormViewModel _viewModel;

    public MemberFormWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _viewModel = new MemberFormViewModel(serviceProvider);
        DataContext = _viewModel;

        // Suscribirse a eventos
        _viewModel.OnSaveCompleted += OnSaveCompleted;
        _viewModel.OnCancelRequested += OnCancelRequested;
    }

    /// <summary>
    /// Indica si se guardo exitosamente
    /// </summary>
    public bool IsSaved { get; private set; }

    /// <summary>
    /// Inicializa para nuevo miembro
    /// </summary>
    public async Task InitializeNewAsync()
    {
        await _viewModel.InitializeNewAsync();
    }

    /// <summary>
    /// Inicializa para editar miembro existente
    /// </summary>
    public async Task InitializeEditAsync(Guid memberId)
    {
        await _viewModel.InitializeEditAsync(memberId);
    }

    private void OnSaveCompleted()
    {
        IsSaved = true;
        DialogResult = true;
        Close();
    }

    private void OnCancelRequested()
    {
        IsSaved = false;
        DialogResult = false;
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        // Desuscribirse de eventos
        _viewModel.OnSaveCompleted -= OnSaveCompleted;
        _viewModel.OnCancelRequested -= OnCancelRequested;

        base.OnClosed(e);
    }
}
