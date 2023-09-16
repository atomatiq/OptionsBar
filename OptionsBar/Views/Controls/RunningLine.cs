using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace OptionsBar.Views.Controls;

public class RunningLine : ContentControl
{
    public static readonly DependencyProperty LineTemplateProperty = DependencyProperty.Register(
        nameof(LineTemplate), typeof(DataTemplate), typeof(RunningLine), new PropertyMetadata(default(DataTemplate)));

    public static readonly DependencyProperty AnimationSpeedProperty = DependencyProperty.Register(
        nameof(AnimationSpeed), typeof(double), typeof(RunningLine), new PropertyMetadata(0.2d));

    public RunningLine()
    {
        Loaded += OnLoaded;
    }

    public DataTemplate LineTemplate
    {
        get => (DataTemplate) GetValue(LineTemplateProperty);
        set => SetValue(LineTemplateProperty, value);
    }

    public double AnimationSpeed
    {
        get => (double) GetValue(AnimationSpeedProperty);
        set => SetValue(AnimationSpeedProperty, value);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var rootPanel = (ItemsControl) Template.FindName("RootPanel", this);
        var rootPanelNext = (ItemsControl) Template.FindName("RootPanelNext", this);

        EvaluatePlaceholders(rootPanel);
        EvaluatePlaceholders(rootPanelNext);

        RunAnimation(rootPanel, rootPanelNext);
    }

    private void EvaluatePlaceholders(ItemsControl panel)
    {
        panel.Items.Add(null);
        panel.UpdateLayout();

        var placeholdersCount = (int) (ActualWidth / panel.ActualWidth);
        var placeholders = Enumerable.Repeat((string) null, placeholdersCount);
        foreach (var item in placeholders) panel.Items.Add(item);
    }

    private async void RunAnimation(ItemsControl panel, ItemsControl panelNext)
    {
        panel.UpdateLayout();
        panelNext.UpdateLayout();

        var animation = new DoubleAnimation
        {
            From = 0,
            To = -panel.ActualWidth,
            RepeatBehavior = RepeatBehavior.Forever
        };

        var animationNext = new DoubleAnimation
        {
            From = ActualWidth,
            To = ActualWidth - panel.ActualWidth,
            RepeatBehavior = RepeatBehavior.Forever
        };

        animation.Duration = TimeSpan.FromMilliseconds((animation.From.Value - animation.To.Value) / AnimationSpeed);
        animationNext.Duration = TimeSpan.FromMilliseconds((animationNext.From.Value - animationNext.To.Value) / AnimationSpeed);
        animationNext.BeginTime = TimeSpan.FromMilliseconds(-animationNext.To.Value / AnimationSpeed);

        panel.BeginAnimation(Canvas.LeftProperty, animation);
        panelNext.BeginAnimation(Canvas.LeftProperty, animationNext);

        await Task.Delay(animationNext.BeginTime.Value);
        panelNext.Visibility = Visibility.Visible;
    }
}