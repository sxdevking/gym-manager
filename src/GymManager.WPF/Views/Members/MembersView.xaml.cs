using System.Windows.Controls;
using GymManager.WPF.ViewModels.Members;

namespace GymManager.WPF.Views.Members;

/// <summary>
/// Vista de lista de miembros
/// </summary>
public partial class MembersView : UserControl
{
    public MembersView()
    {
        InitializeComponent();

        // Cargar miembros cuando se cargue la vista
        Loaded += async (s, e) =>
        {
            if (DataContext is MembersViewModel vm)
            {
                await vm.LoadMembersAsync();
            }
        };
    }
}
