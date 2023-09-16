using OptionsBar.ViewModels;

namespace OptionsBar.Views;

public partial class OptionsView
{
    public OptionsView(OptionsViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}