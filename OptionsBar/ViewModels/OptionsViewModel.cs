using CommunityToolkit.Mvvm.ComponentModel;

namespace OptionsBar.ViewModels;

public partial class OptionsViewModel : ObservableObject
{
    [ObservableProperty] private double _offset;
    [ObservableProperty] private string[] _constraints;
}