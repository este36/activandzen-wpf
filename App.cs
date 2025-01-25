using System.Configuration;
using System.Data;
using System.Windows;

namespace ActivAndZen;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    [STAThread] // Required for WPF apps
    public static void Main()
    {
        var app = new App();
        var mainWindow = new MainWindow();
        app.Run(mainWindow); // Starts the main UI loop
    }
}

