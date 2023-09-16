using OptionsBar.ViewModels;

namespace OptionsBar.Views;

public partial class UtilsView
{
    public UtilsView(UtilsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}