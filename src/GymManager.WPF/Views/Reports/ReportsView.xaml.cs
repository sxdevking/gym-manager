using GymManager.WPF.ViewModels.Reports;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Reports;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is ReportsViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
