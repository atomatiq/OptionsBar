using Autodesk.Revit.Attributes;
using Nice3point.Revit.Toolkit.External;
using OptionsBar.Views;
using OptionsBar.Views.Utils;

namespace OptionsBar.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowRunningBarCommand : ExternalCommand
{
    public override void Execute()
    {
        RibbonController.ShowOptionsBar(new RunningLineView());
        RibbonController.HideOptionsBar(TimeSpan.FromSeconds(30));
    }
}