using System.Windows;
using GToonManager.Models;

namespace GToonManager.Services;

public class ThemeService
{
    public static void ApplyTheme(Theme theme)
    {
        var app = Application.Current;
        if (app?.Resources == null) return;

        ResourceDictionary themeResources;
        
        switch (theme)
        {
            case Theme.Dark:
                themeResources = (ResourceDictionary)app.Resources["DarkTheme"];
                break;
            case Theme.Light:
            default:
                themeResources = (ResourceDictionary)app.Resources["LightTheme"];
                break;
        }

        if (themeResources != null)
        {
            // Update each theme color in the application resources
            foreach (var key in themeResources.Keys)
            {
                if (app.Resources.Contains(key))
                {
                    app.Resources[key] = themeResources[key];
                }
            }
        }

        // Force all windows to refresh their styles
        foreach (Window window in app.Windows)
        {
            RefreshWindow(window);
        }
    }

    private static void RefreshWindow(Window window)
    {
        // Force the window to re-evaluate its resources
        window.UpdateLayout();
        
        // Recursively refresh all child elements
        RefreshVisualTree(window);
    }

    private static void RefreshVisualTree(DependencyObject parent)
    {
        if (parent == null) return;

        int childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childCount; i++)
        {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(parent, i);
            
            // Force re-evaluation of resources for this element
            if (child is FrameworkElement element)
            {
                element.InvalidateVisual();
                element.UpdateLayout();
            }
            
            // Recursively refresh children
            RefreshVisualTree(child);
        }
    }
} 