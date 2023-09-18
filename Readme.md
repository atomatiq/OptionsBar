# OptionsBar: Enhancing the User Experience in Revit

[Revit](https://www.autodesk.com/products/revit/overview) is a powerful tool for building design and modeling, and undoubtedly, high efficiency in working with this software is a key aspect for successful projects. 
One of the tools that contributes to this efficiency is the OptionsBar, but not many people are aware of its capabilities and how to configure it.

# Introduction to OptionsBar

[OptionsBar](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-28BDE98C-E8A9-4C74-8ABC-9DABD13163D9) is a convenient toolbar in Revit that has tremendous potential. 
It provides capabilities ranging from adding your own tools to the toolbar to displaying information about the operation of your own add-ins. 
However, Revit API documentation does not provide enough information on how to use OptionsBar to its full extent.

![](https://help.autodesk.com/cloudhelp/2024/ENU/Revit-GetStarted/images/GUID-CCF99547-F6D8-4A0F-908C-49BA85BF3C49.png)

# Getting Started

Before we begin customizing the OptionsBar, we'll need to add the necessary dependencies and create a controller to manage this panel.

Technologies Used:

- Language: C#
- UI framework: WPF

# Configuring OptionsBar. Step by Step

Let's delve into the process of configuring OptionsBar and how to do it most efficiently.

## Step 1: Adding Dependencies

Understanding that the user interface in Revit is implemented using WPF (Windows Presentation Foundation) technology helps us access all the interface elements. 
For this, we'll need the `AdWindows.dll` library, which comes with Revit.

To compile the project on any device, let's add a NuGet package to the `.csproj` project file as follows:

```xml
<ItemGroup>
    <PackageReference Include="Nice3point.Revit.Api.AdWindows" Version="$(RevitVersion).*"/>
</ItemGroup>
```

## Step 2: Creating a Controller

To control OptionsBar, we'll need a controller that handles its display and hiding. 
To access the Revit ribbon and OptionsBar, we'll use the static property `ComponentManager.Ribbon` from the `AdWindows.dll` library that we included in the previous step.

The process of configuring OptionsBar can be divided into the following stages:

1. Finding the built-in panel.
2. Creating a custom panel.
3. Hiding/showing the built-in and custom panels.

It's important to note that we don't remove or replace the built-in OptionsBar panel to avoid disrupting Revit's functionality. 
Instead, we simply hide it and display our custom panel when necessary.

Let's take a look at a code example that allows displaying and hiding the custom panel:

```C#
public static class RibbonController
{
    private static readonly Grid RootGrid;
    private static ContentPresenter _panelPresenter;
    private static readonly FrameworkElement InternalToolPanel;

    static RibbonController()
    {
        RootGrid = VisualUtils.FindVisualParent<Grid>(ComponentManager.Ribbon, "rootGrid");
        if (RootGrid is null) throw new InvalidOperationException("Cannot find root grid in Revit UI");

        InternalToolPanel = VisualUtils.FindVisualChild<DialogBarControl>(RootGrid, string.Empty);
        if (InternalToolPanel is null) throw new InvalidOperationException("Cannot find internal tool panel in Revit UI");
    }

    public static void ShowOptionsBar(FrameworkElement content)
    {
        if (_panelPresenter is not null)
        {
            _panelPresenter.Content = content;
            _panelPresenter.Visibility = Visibility.Visible;
            InternalToolPanel.Height = 0;
            return;
        }

        _panelPresenter = CreateOptionsBar();
        _panelPresenter.Content = content;

        InternalToolPanel.Height = 0;
    }

    public static void HideOptionsBar()
    {
        if (_panelPresenter is null) return;

        _panelPresenter.Content = null;
        _panelPresenter.Visibility = Visibility.Collapsed;

        InternalToolPanel.Height = 26;
    }

    private static ContentPresenter CreateOptionsBar()
    {
        const int panelRow = 2;

        RootGrid.RowDefinitions.Insert(2, new RowDefinition
        {
            Height = new GridLength(1, GridUnitType.Auto)
        });

        foreach (UIElement child in RootGrid.Children)
        {
            var row = Grid.GetRow(child);
            if (row > 1) Grid.SetRow(child, row + 1);
        }

        var panelPresenter = new ContentPresenter();
        Grid.SetRow(panelPresenter, panelRow);
        RootGrid.Children.Add(panelPresenter);

        return panelPresenter;
    }
}
```

In the example, only public properties of the Revit API and system methods of WPF are used. 
Interaction with the ribbon is implemented with the `ShowOptionsBar()` and `HideOptionsBar()` methods. 
The `ShowOptionsBar()` method takes any FrameworkElement, which will be displayed to the user.

## Step 3: Creating a User Interface

To create a user interface for OptionsBar, we'll use the MVVM (Model-View-ViewModel) pattern and the WPF (Windows Presentation Foundation) framework.

For example, let's consider a simple panel layout with a text field and a dropdown list:

![](https://github.com/Nice3point/OptionsBar/assets/20504884/3932ef5f-82e0-4b59-bc61-b95aef6199c8)

```xml
<StackPanel
        x:Class="OptionsBar.Views.OptionsView"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:viewModels="clr-namespace:OptionsBar.ViewModels"
        d:DataContext="{d:DesignInstance Type=viewModels:OptionsViewModel}"
        mc:Ignorable="d"
        Background="#FFE5F0D7"
        Orientation="Horizontal"
        Height="26"
        d:DesignWidth="430">
    <TextBlock
            Margin="10 0 0 0"
            Text="Wall options"
            VerticalAlignment="Center" />
    <Border
            Width="3"
            BorderThickness="1 0"
            BorderBrush="Azure"
            Background="Gray"
            Margin="10 0" />
    <TextBlock
            Text="Offset: "
            VerticalAlignment="Center" />
    <TextBox
            Width="100"
            Margin="10 1 0 1"
            VerticalContentAlignment="Center"
            Text="{Binding Offset, UpdateSourceTrigger=PropertyChanged}" />
    <TextBlock
            Text="Constraint: "
            Margin="10 0 0 0"
            VerticalAlignment="Center" />
    <ComboBox
            Width="100"
            Margin="10 1 0 1"
            VerticalContentAlignment="Center"
            SelectedIndex="0"
            ItemsSource="{Binding Constraints}" />
</StackPanel>
```

Sample code for the ViewModel:

```C#
public partial class OptionsViewModel : ObservableObject
{
    [ObservableProperty] private double _offset;
    [ObservableProperty] private string[] _constraints;
}
```

The layout is no different from regular WPF applications, but the root element will be a container like StackPanel, Grid, and so on, since OptionBar is embedded in the Revit UI.

## Use Cases

OptionsBar provides endless possibilities for streamlining the workflow in Revit. 
Let's explore some of the most popular use cases:

### Use Case 1: Utilities

OptionsBar can be used to add custom tools that can be conveniently placed on the toolbar.
This is especially useful when creating a separate window is unnecessary.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/10cafa43-8255-49ba-b764-7c5c0be2340a)

### Use Case 2: Element Selection Options

This scenario covers situations where a user is working with a model in Revit and needs to select a specific element for further editing. 
For more convenient and intuitive parameter customization, you can use OptionsBar instead of creating additional windows.

Example: Suppose you have a plugin that allows users to adjust the top offset of a wall. Let's see how to arrange all the tools on the OptionsBar panel.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/65a508ce-2f9c-414d-80b3-f996b5e679d6)

- Step 1: Adding OptionsBar to the Revit Ribbon

  The first step in this scenario is to set up OptionsBar for display on the Revit ribbon. 
  You can create your own view for OptionsBar with parameters that the user can customize.
  
  Example code using the MVVM pattern:
    
    ```C#
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
    ```

- Step 2: Selecting an Element

  After configuring OptionsBar, the user selects an element in the model with which they want to work. 
  This could be, for example, selecting a wall to adjust the offset.
  
  Example code for element selection:
  
  ```C#
  private Wall PickWall()
  {
      var selectionConfiguration = new SelectionConfiguration().Allow.Element(selection => selection is Wall);
      var reference = UiDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter, "Select wall");
      return reference.ElementId.ToElement<Wall>(Document);
  }
  ```

- Step 3: Adjusting Element Parameters

  Now that the user has selected an element and OptionsBar is configured, you can start adjusting the element's parameters. 
  We'll take the offset value from the ViewModel entered by the user and set it to the wall parameter.

  Example code for adjusting element parameters:
  
  ```C#
  private void ModifyWall(Wall wall, OptionsViewModel options)
  {
      using var transaction = new Transaction(Document);
      transaction.Start("Set offset");
      wall.GetParameter(BuiltInParameter.WALL_TOP_OFFSET)!.Set(options.Offset);
      transaction.Commit();
  }
  ```

- Step 4: Restoring the Revit Ribbon

  After completing the element parameter configuration, it's important to hide OptionsBar and restore the normal state of the Revit ribbon.
  To do this, add a `finally` block after calling all the methods.

  ```C#
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
  ```

  This scenario allows users to quickly and conveniently customize the parameters of model elements without the need to open additional windows or perform extra actions. 
  OptionsBar makes the process of editing elements more intuitive and efficient.

### Use Case 3: Marquee

Want something unusual? A marquee can add excitement to your mundane modeling routine.

![](https://github.com/Nice3point/OptionsBar/assets/20504884/9427569b-1cce-41c8-a1f5-e88bea4c8683)

## Conclusion

OptionsBar is a powerful tool in Revit that allows you to optimize your workflow and make it more efficient. 
Don't limit yourself to the described use cases — experiment, create your unique solutions, and make working in Revit even more productive.

Project source code: [GitHub](https://github.com/atomatiq/OptionsBar)

Installers with examples for Revit are also available: [Releases](https://github.com/atomatiq/OptionsBar/releases)