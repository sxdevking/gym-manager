using GymManager.WPF.ViewModels.Classes;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Classes;

public partial class ClassesView : UserControl
{
    public ClassesView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is ClassesViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
