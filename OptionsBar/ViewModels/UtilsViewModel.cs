using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OptionsBar.Views.Utils;

namespace OptionsBar.ViewModels;

public partial class UtilsViewModel : ObservableObject
{
    [ObservableProperty] private string _message;

    [RelayCommand]
    private void ShowDialog()
    {
        TaskDialog.Show("Message", Message);
        RibbonController.HideOptionsBar();
    }
}