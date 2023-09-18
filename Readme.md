# OptionsBar: Улучшение пользовательского опыта в Revit

[Revit](https://www.autodesk.com/products/revit/overview) - это мощное инструментальное средство для проектирования и моделирования зданий, и, безусловно, высокая эффективность работы с этим программным обеспечением является ключевым аспектом для успешных проектов. 
Одним из инструментов, способствующих этой эффективности, является OptionsBar, но мало кто знает о его возможностях и том как его настроить.

# Введение в OptionsBar

[OptionsBar](https://help.autodesk.com/view/RVT/2024/ENU/?guid=GUID-28BDE98C-E8A9-4C74-8ABC-9DABD13163D9) - это удобная панель инструментов в Revit, которая обладает огромным потенциалом. 
Она предоставляет возможности, начиная от добавления собственных инструментов на ленту инструментов и заканчивая отображением информации о работе ваших собственных надстроек. 
Однако, документация по Revit API не предоставляет достаточно информации о том, как использовать OptionsBar в полной мере.

![](https://help.autodesk.com/cloudhelp/2024/ENU/Revit-GetStarted/images/GUID-CCF99547-F6D8-4A0F-908C-49BA85BF3C49.png)

# Начало работы

Прежде чем мы начнем настраивать OptionsBar, нам потребуется добавить необходимые зависимости и создать контроллер для управления этой панелью.

Использованные технологии:

- Язык - C#
- UI framework - WPF

# Настройка OptionsBar. Шаг за Шагом

Давайте подробно рассмотрим процесс настройки OptionsBar и как это можно сделать максимально эффективным образом.

## Шаг 1: Добавление Зависимостей

Понимание того, что пользовательский интерфейс в Revit реализован с использованием технологии WPF (Windows Presentation Foundation), помогает нам получить доступ ко всем элементам интерфейса.
Для этого нам понадобится библиотека `AdWindows.dll`, которая поставляется с Revit. 

Для компиляции проекта на любом устройстве добавим NuGet пакет в .csproj файл проекта следующим образом:

```xml
<ItemGroup>
    <PackageReference Include="Nice3point.Revit.Api.AdWindows" Version="$(RevitVersion).*"/>
</ItemGroup>
```

## Шаг 2: Создание Контроллера

Для того чтобы управлять OptionsBar, нам потребуется контроллер, который будет обрабатывать его отображение и скрытие. Для доступа к ленте инструментов Revit и OptionsBar мы воспользуемся статическим свойством `ComponentManager.Ribbon` из библиотеки `AdWindows.dll`, которую мы подключили на предыдущем шаге.

Процесс настройки OptionsBar можно разбить на следующие этапы:

1. Поиск встроенной панели
2. Создание пользовательской панели
3. Скрытие/отображение встроенной и пользовательской панели

Важно заметить, что мы не удаляем или не заменяем встроенную панель OptionsBar, чтобы не нарушить функциональность Revit. 
Вместо этого мы просто скрываем ее и отображаем собственную панель, когда это необходимо.

Давайте рассмотрим пример кода, который позволяет отображать и скрывать пользовательскую панель:

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

В примере использованы только публичные свойства RevitApi и системные методы WPF.
Взаимодействие с лентой реализовано  методами `ShowOptionsBar()` и `HideOptionsBar()`. Метод `ShowOptionsBar()` принимает любой FrameworkElement, который и будет отображаться для пользователя.

## Шаг 3: Создание пользовательского интерфейса

Чтобы создать пользовательский интерфейс для OptionsBar, мы будем использовать паттерн MVVM (Model-View-ViewModel) и фреймворк WPF (Windows Presentation Foundation).

Для примера, рассмотрим простую разметку панели с текстовым полем и выпадающим списком:

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/3932ef5f-82e0-4b59-bc61-b95aef6199c8)

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

Пример кода для ViewModel:

```C#
public partial class OptionsViewModel : ObservableObject
{
    [ObservableProperty] private double _offset;
    [ObservableProperty] private string[] _constraints;
}
```

Верстка ничем не отличается от привычных WPF приложений, однако корневым элементов будет не Window, а любой контейнер по типу StackPanel, Grid и т.д., поскольку OptionBar является встраиваемым в Revit UI.

## Сценарии использования

OptionsBar предоставляет бесконечные возможности для оптимизации рабочего процесса в Revit. Рассмотрим некоторые из наиболее популярных сценариев использования:

### Сценарий 1: Утилиты

OptionsBar может быть использован для добавления пользовательских инструментов, которые могут быть удобно расположены на ленте инструментов. 
Это особенно полезно, когда создание отдельного окна излишне.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/10cafa43-8255-49ba-b764-7c5c0be2340a)

### Сценарий 2: Опции при выборе элемента в модели

Этот сценарий охватывает ситуации, когда пользователь работает с моделью в Revit и должен выбрать определенный элемент для последующего редактирования. 
Для более удобной и интуитивно понятной настройки параметров вы можете использовать OptionsBar, вместо создания дополнительных окон.

Пример: Допустим, у вас есть плагин, который позволяет пользователю настраивать смещение сверху стены. Рассмотрим как расположить все инструменты на панели OptionsBar.

![изображение](https://github.com/Nice3point/OptionsBar/assets/20504884/65a508ce-2f9c-414d-80b3-f996b5e679d6)

- Шаг 1: Добавление OptionsBar на ленту Revit

    Первым шагом в этом сценарии является настройка OptionsBar для отображения на ленте инструментов Revit.
    Вы можете создать собственное представление (View) для OptionsBar, с параметрами, которые пользователь может настраивать.
    
    Пример кода, с использованием паттерна MVVM:
    
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

- Шаг 2: Выбор элемента

  После настройки OptionsBar пользователь выбирает элемент в модели, с которым он хочет работать. Это может быть, например, выбор стены для настройки смещения.
  
  Пример кода для выбора элемента:
  
  ```C#
  private Wall PickWall()
  {
      var selectionConfiguration = new SelectionConfiguration().Allow.Element(selection => selection is Wall);
      var reference = UiDocument.Selection.PickObject(ObjectType.Element, selectionConfiguration.Filter, "Select wall");
      return reference.ElementId.ToElement<Wall>(Document);
  }
  ```

- Шаг 3: Изменение параметров элемента

  Теперь, когда пользователь выбрал элемент и OptionsBar настроен, можно приступить к настройке параметров элемента. 
  Значение смещение мы возьмем из ViewModel, которое ввел пользователь и зададим его параметру стены.
  
  Пример кода для изменения параметров элемента:
  
  ```C#
  private void ModifyWall(Wall wall, OptionsViewModel options)
  {
      using var transaction = new Transaction(Document);
      transaction.Start("Set offset");
      wall.GetParameter(BuiltInParameter.WALL_TOP_OFFSET)!.Set(options.Offset);
      transaction.Commit();
  }
  ```

- Шаг 4: Восстановление ленты Revit

  После завершения настройки параметров элемента, важно скрыть OptionsBar и восстановить нормальное состояние ленты инструментов Revit. Для этого добавим `finally` блок после вызова всех методов.
  
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
  
  Этот сценарий позволяет пользователям быстро и удобно настраивать параметры элементов модели без необходимости открывать дополнительные окна или выполнять лишние действия. OptionsBar делает процесс редактирования элементов более интуитивным и эффективным.

### Сценарий 3: Бегущая строка

Хотите чего-то необычного? Бегущая строка разбавит ваши серые будни моделирования

![](https://github.com/Nice3point/OptionsBar/assets/20504884/9427569b-1cce-41c8-a1f5-e88bea4c8683)

## Заключение

OptionsBar - это мощный инструмент в Revit, который позволяет оптимизировать рабочий процесс и сделать его более эффективным. 
Не ограничивайтесь только описанными сценариями использования - экспериментируйте, создавайте свои уникальные решения и сделайте работу в Revit еще более продуктивной.

Исходный код проекта: https://github.com/Nice3point/OptionsBar

Также доступны установщики с примерами для Revit: https://github.com/Nice3point/OptionsBar/releases