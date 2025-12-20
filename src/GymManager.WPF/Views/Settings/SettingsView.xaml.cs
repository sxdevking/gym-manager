using GymManager.WPF.ViewModels.Settings;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Settings;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is SettingsViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
