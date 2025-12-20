using GymManager.WPF.ViewModels.Payments;
using System.Windows.Controls;

namespace GymManager.WPF.Views.Payments;

public partial class PaymentsView : UserControl
{
    public PaymentsView()
    {
        InitializeComponent();
        Loaded += async (s, e) =>
        {
            if (DataContext is PaymentsViewModel vm)
            {
                await vm.InitializeAsync();
            }
        };
    }
}
