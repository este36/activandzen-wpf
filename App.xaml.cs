using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace ActivAndZen;


public static class MouseTracker
{
    private static UIElement? lastMouseDown;

    public static void previewMouseDown(object sender, MouseButtonEventArgs e)
    {
        lastMouseDown = (UIElement)e.OriginalSource;
    }

    private static bool WasDown(UIElement e)
    {
        return e == lastMouseDown;
    }
    private static void Reset()
    {
        lastMouseDown = null;
    }

    public static MouseButtonEventHandler DefineClick(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        return (sender, e) =>
        {
            if (MouseTracker.WasDown((UIElement)e.OriginalSource))
            {
               action();
            }
            MouseTracker.Reset();
        };
    }
}

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
    }

    public static object GetResource(string xKey)
    {
        return Application.Current.Resources[xKey];
    }
}
