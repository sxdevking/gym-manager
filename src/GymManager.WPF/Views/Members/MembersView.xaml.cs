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
    }

    /// <summary>
    /// Se llama cuando el DataContext cambia
    /// </summary>
    public async void Initialize()
    {
        if (DataContext is MembersViewModel viewModel)
        {
            await viewModel.LoadMembersAsync();
        }
    }
}