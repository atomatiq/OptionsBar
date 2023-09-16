using System.Windows;
using System.Windows.Media;

namespace OptionsBar.Views.Utils;

public static class VisualUtils
{
    [CanBeNull]
    public static T FindVisualParent<T>(FrameworkElement element, string name) where T : FrameworkElement
    {
        var parentElement = (FrameworkElement) VisualTreeHelper.GetParent(element);
        while (parentElement != null)
        {
            if (parentElement is T parent)
                if (parentElement.Name == name)
                    return parent;

            parentElement = (FrameworkElement) VisualTreeHelper.GetParent(parentElement);
        }

        return null;
    }

    public static T FindVisualChild<T>(FrameworkElement element, string name) where T : Visual
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            var childElement = (FrameworkElement) VisualTreeHelper.GetChild(element, i);
            if (childElement is T child)
                if (childElement.Name == name)
                    return child;

            var descendent = FindVisualChild<T>(childElement, name);
            if (descendent != null) return descendent;
        }

        return null;
    }
}