using GymManager.WPF.Helpers;
using GymManager.WPF.ViewModels.Members;
using System.Windows;
using System.Windows.Input;

namespace GymManager.WPF.Views.Members;

/// <summary>
/// Ventana modal para agregar/editar miembros
/// Diseño Glass/Acrylic con efecto transparente
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

        // Permitir arrastrar la ventana desde el header
        Loaded += OnWindowLoaded;
        MouseLeftButtonDown += OnMouseLeftButtonDown;
    }

    /// <summary>
    /// Indica si se guardo exitosamente
    /// </summary>
    public bool IsSaved { get; private set; }

    /// <summary>
    /// Aplica efectos visuales al cargar
    /// </summary>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Intentar aplicar efecto Mica/Acrylic (Windows 11)
            WindowHelper.ApplyMica(this, useDarkMode: true);
        }
        catch
        {
            // Si falla, la ventana usa el fondo gradient definido en XAML
            System.Diagnostics.Debug.WriteLine("Mica no disponible, usando fondo alternativo");
        }
    }

    /// <summary>
    /// Permite arrastrar la ventana desde cualquier parte del header
    /// </summary>
    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            // Solo arrastrar si el click fue en el area superior (header)
            var clickPoint = e.GetPosition(this);
            if (clickPoint.Y < 120) // Area del header aproximadamente
            {
                DragMove();
            }
        }
    }

    /// <summary>
    /// Minimiza la ventana
    /// </summary>
    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    /// <summary>
    /// Cierra la ventana
    /// </summary>
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        OnCancelRequested();
    }

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

    /// <summary>
    /// Cierra la ventana con Escape
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            OnCancelRequested();
        }
        base.OnKeyDown(e);
    }
}