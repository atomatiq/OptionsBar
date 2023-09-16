using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Nice3point.Revit.Toolkit.External;
using Nice3point.Revit.Toolkit.Options;
using OptionsBar.ViewModels;
using OptionsBar.Views;
using OptionsBar.Views.Utils;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;

namespace OptionsBar.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ShowBarCommand : ExternalCommand
{
    public override void Execute()
    {
        try
        {
            var options = SetupOptionsBar();
            var wall = PickWall();
            ModifyWall(wall, options);
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
        finally
        {
            RibbonController.HideOptionsBar();
        }
    }

    private OptionsViewModel SetupOptionsBar()
    {
        var options = new OptionsViewModel
        {
            Offset = 0,
            Constraints = Document.EnumerateInstances<Level>(BuiltInCategory.OST_Levels).Select(level => level.Name).ToArray()
        };

        var view = new OptionsView(options);
        RibbonController.ShowOptionsBar(view);
        return options;
    }

    private Wall PickWall()
    {
        var selectionConfiguration = new SelectionConfiguration().Allow.Element(selection => selection is Wall);
        var reference = UiDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter, "Select wall");
        return reference.ElementId.ToElement<Wall>(Document);
    }

    private void ModifyWall(Wall wall, OptionsViewModel options)
    {
        // Example of using user-defined values from the Options Bar
        using var transaction = new Transaction(Document);
        transaction.Start("Set offset");
        wall.GetParameter(BuiltInParameter.WALL_TOP_OFFSET)!.Set(options.Offset);
        transaction.Commit();
    }
}