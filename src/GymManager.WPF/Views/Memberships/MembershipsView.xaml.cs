using GymManager.WPF.ViewModels.Memberships;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Memberships;

public partial class MembershipsView : UserControl
{
    public MembershipsView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is MembershipsViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
