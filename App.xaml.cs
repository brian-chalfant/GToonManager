using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using GToonManager.Views;

namespace GToonManager;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        // Show splash screen
        await LoadingWindow.ShowSplashScreen();
        
        // Continue with normal startup
        base.OnStartup(e);
    }
}

