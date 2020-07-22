using System.ComponentModel;
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging.Serilog;
using Avalonia.ReactiveUI;
using Microsoft.Extensions.DependencyInjection;
using LibFloaderClient.Interfaces.Logger;
using FloaderClientGUI.GUISpecific.Logger;
using LibFloaderClient.Interfaces.SerialPortsLister;
using LibFloaderClient.Implementations.SerialPortsLister;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Implementations.Device;
using LibFloaderClient.Interfaces.Versioned.Common;
using LibFloaderClient.Implementations.Versioned.Common;

namespace FloaderClientGUI
{
   public class Program
    {
        /// <summary>
        /// Dependency injection service provider
        /// </summary>
        public static ServiceProvider Di {get; set; }

        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        public static void Main(string[] args)
        {
            // Preparing DI
            Di = ConfigureServices()
                .BuildServiceProvider();

            BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseReactiveUI();

        // Setting up DI
        public static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<ILogger, Logger>();
            services.AddSingleton<ISerialPortsLister, SerialPortsLister>();
            services.AddSingleton<IDeviceIdentifier, DeviceIdentifier>();
            services.AddSingleton<IVersionValidator, VersionValidator>();

            return services;
        }

        // Getting main window
        public static Window GetMainWindow()
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
            {
                return desktopLifetime.MainWindow;
            }
            return null;
        }
    }
}
