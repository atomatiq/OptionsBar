using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using OptionsBar.ViewModels;
using OptionsBar.Views;
using OptionsBar.Views.Utils;

namespace OptionsBar.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowUtilsBarCommand : ExternalCommand
{
    public override void Execute()
    {
        var viewModel = new UtilsViewModel();
        var view = new UtilsView(viewModel);
        RibbonController.ShowOptionsBar(view);
    }
}