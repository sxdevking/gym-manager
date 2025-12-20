using GymManager.WPF.ViewModels.Sales;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Sales;

public partial class SalesView : UserControl
{
    public SalesView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is SalesViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
