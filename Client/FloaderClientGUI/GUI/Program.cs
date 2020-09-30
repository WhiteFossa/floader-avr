using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.ReactiveUI;
using FloaderClientGUI.GUISpecific.Logger;
using LibFloaderClient.Implementations.Auxiliary;
using LibFloaderClient.Implementations.DAO;
using LibFloaderClient.Implementations.Device;
using LibFloaderClient.Implementations.SerialPortsLister;
using LibFloaderClient.Implementations.Versioned.Common;
using LibFloaderClient.Implementations.Versioned.Driver;
using LibFloaderClient.Interfaces.Auxiliary;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.SerialPortsLister;
using LibFloaderClient.Interfaces.Versioned.Common;
using LibFloaderClient.Interfaces.Versioned.Driver;
using LibIntelHex.Implementations;
using LibIntelHex.Implementations.Reader;
using LibIntelHex.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FloaderClientGUI
{
    public class Program
    {
        /// <summary>
        /// Program name
        /// </summary>
        public const string AppName = "Fossa's loader";

        /// <summary>
        /// Program version
        /// </summary>
        public const string AppVersion = "0.0.1";

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
            services.AddSingleton<IDao, Dao>();
            services.AddSingleton<IDeviceDataGetter, DeviceDataGetter>();
            services.AddSingleton<IDeviceDriverV1, DeviceDriverV1>();
            services.AddSingleton<IDeviceIndependentOperationsProvider, DeviceIndependentOperationsProvider>();
            services.AddSingleton<IFilenamesGenerator, FilenamesGenerator>();

            // Dependencies for LibIHEX
            services.AddSingleton<IBytesReaderWriter, BytesReaderWriter>();
            services.AddSingleton<IChecksumProcessor, ChecksumProcessor>();
            services.AddSingleton<IHexWriter, HexWriter>();
            services.AddSingleton<IRecordFormatter, RecordFormatter>();
            services.AddSingleton<IHexReader, HexReader>();

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

        /// <summary>
        /// Get full application name
        /// </summary>
        public static string GetFullAppName()
        {
            return $"{ AppName } v{AppVersion}";
        }
    }
}
