using Nice3point.Revit.Toolkit.External;
using OptionsBar.Commands;

namespace OptionsBar;

[UsedImplicitly]
public class Application : ExternalApplication
{
    public override void OnStartup()
    {
        CreateRibbon();
    }

    private void CreateRibbon()
    {
        var panel = Application.CreatePanel("Commands", "OptionsBar");

        var showSelectionButton = panel.AddPushButton<ShowBarCommand>("Select\nWall");
        showSelectionButton.SetImage("/OptionsBar;component/Resources/Icons/RibbonIcon16.png");
        showSelectionButton.SetLargeImage("/OptionsBar;component/Resources/Icons/RibbonIcon32.png");

        var showUtilsButton = panel.AddPushButton<ShowUtilsBarCommand>("Show\nUtils");
        showUtilsButton.SetImage("/OptionsBar;component/Resources/Icons/RibbonIcon16.png");
        showUtilsButton.SetLargeImage("/OptionsBar;component/Resources/Icons/RibbonIcon32.png");

        var showRunningButton = panel.AddPushButton<ShowRunningBarCommand>("Show\nrunning line");
        showRunningButton.SetImage("/OptionsBar;component/Resources/Icons/RibbonIcon16.png");
        showRunningButton.SetLargeImage("/OptionsBar;component/Resources/Icons/RibbonIcon32.png");
    }
}