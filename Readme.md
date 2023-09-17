# Options Bar

В Revit есть довольно интересная панель инструментов [OptionsBar](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-28BDE98C-E8A9-4C74-8ABC-9DABD13163D9), расположенная под основной
лентой.

![](https://help.autodesk.com/cloudhelp/2024/ENU/Revit-GetStarted/images/GUID-CCF99547-F6D8-4A0F-908C-49BA85BF3C49.png)

Она может быть использована для различных задач, начиная от добавления собственных инструментов на ленту, заканчивая отображением индикации процесса для нашей надстройки.

Однако в документации по Revit API нет сведений по ее использованию. 
Тем не менее решение есть. В этой статье мы рассмотрим как добавить инструменты на панель OptionsBar, как отобразить ее на ленте и сделать для нее свой пользовательский интерфейс.

Использованные технологии:
- Язык - C#
- UI framework - WPF

## Добавление зависимостей

Ни для кого не секрет, что пользовательский интерфейс в Revit написан на WPF. Поэтому мы легко можем получить доступ ко всем элементам интерфейса.

Нам понадобится библиотека `AdWindows.dll`, она официальная и устанавливается вместе с Revit. Мы подключим Nuget пакет, чтобы проект компилировался на любом устройстве.

Добавьте эти строки в .csproj файл проекта:

```xml
<ItemGroup>
    <PackageReference Include="Nice3point.Revit.Api.AdWindows" Version="$(RevitVersion).*"/>
</ItemGroup>
```

## Создание контроллера 

Для доступа к ленте в Revit есть статическое свойство `ComponentManager.Ribbon`, из библиотеки `AdWindows.dll` которую мы подключили ранее. Им мы и воспользуемся для получения ссылки на `OptionsBar`.

Весь процесс добавления собственной панели состоит из нескольких этапов:
1. Поиск встроенной панели
2. Создание пользовательской панели
3. Управление видимостью встроенной / пользовательской панелью

Важно то, что мы не будем удалять или заменять встроенную панель, иначе нарушим функционал Revit, мы будем скрывать ее, и отображать собственную когда это необходимо.

Далее мы напишем код, который будет отображать и закрывать панель.
Взаимодействие с лентой будет выполняться методами `ShowOptionsBar()` и `HideOptionsBar()`.

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

Как видите мы используем только публичные свойства RevitApi и системные методы WPF.

## Создание пользовательского интерфейса

Для создания интерфейса мы будем использовать паттерн MVVM и WPF framework.

Верстка ничем не отличается от привычных WPF приложений, однако корневым элементов будет не Window, а любой контейнер по типу StackPanel, Grid и т.д., поскольку наш OptionBar является встраиваемым в UI Revit.

Ниже представлена верстка панели на который расположено поле ввода и выпадающий список.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/6a3cd5e3-8d43-4082-b3d7-4e25c08dce78)

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

## Сценарии использования

### Утилиты

OptionsBar может быть использован для добавления пользовательских инструментов, которые можно расположить в удобном под рукой месте.
Это может быть полезно, когда создание отдельного окна избыточно. 
![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/d1fc6840-302d-45df-8995-0bd77595f61c)

### Опции при выборе элемента в модели

OptionsBar так же помогает задать параметры для ваших инструментов во время PickObject операции. 
Например, ваш плагин должен задавать уклон для выбранной трубы и, вместо того чтобы делать отдельное окно с одним полем ввода, вы можете воспользоваться OptionsBar.

В следующем примере мы сделаем команду, которая будет задавать смещение сверху для выбранной стены.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/ed82eded-9e41-4841-a073-b27989311178)

Разобьем команду на несколько этапов, каждый из которых поместим в метод:

1. Добавление OptionsBar на ленту Revit

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

В этом случае мы создаем новый View, задаем значения для ViewModel и с помощью команды ShowOptionsBar отобразим нашу панель на ribbon.

2. Выбор элемента

```C#
private Wall PickWall()
{
    var selectionConfiguration = new SelectionConfiguration().Allow.Element(selection => selection is Wall);
    var reference = UiDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter, "Select wall");
    return reference.ElementId.ToElement<Wall>(Document);
}
```

3. Изменение параметров элемента

```C#
private void ModifyWall(Wall wall, OptionsViewModel options)
{
    using var transaction = new Transaction(Document);
    transaction.Start("Set offset");
    wall.GetParameter(BuiltInParameter.WALL_TOP_OFFSET)!.Set(options.Offset);
    transaction.Commit();
}
```

Далее мы должны объединить методы выше в методе `Execute`, который является точкой входа в команду.

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

В этой ситуации OptionsBar будет отображаться на протяжении всей `PickObject` операции, и до выбора элемента в модели указать смещении и базовый уровень.

Важно после выбора элемента скрыть OptionsBar, и восстановить Revit ribbon в прежнее состояние, для этого поместим `HideOptionsBar` метод в finally блок.

### Бегущая строка

Хотите чего-то необычного? Бегущая строка разбавит ваши серые будни моделирования

![](https://github.com/Nice3point/OptionsBar/assets/20504884/9427569b-1cce-41c8-a1f5-e88bea4c8683)

----

На этом возможности OptionsBar не заканчиваются, вы можете придумать собственные варианты его использования и сделать удобство пользования Revit еще лучше.

Исходный код проекта вы можете найти на GitHub: https://github.com/Nice3point/OptionsBar


