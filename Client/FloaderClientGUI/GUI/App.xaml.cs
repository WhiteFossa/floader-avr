using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FloaderClientGUI.ViewModels;
using FloaderClientGUI.Views;
using FloaderClientGUI.Models;

namespace FloaderClientGUI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Root model
                var model = new MainModel();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(model),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}