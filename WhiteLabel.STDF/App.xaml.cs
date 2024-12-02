using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using WhiteLabel.STDF.Views;

namespace WhiteLabel.STDF;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private IHost _host;
	public T GetService<T>() where T : class =>
		_host.Services.GetService(typeof(T)) as T;

	public App() { }

	private async void OnStartup(object sender, StartupEventArgs e)
	{
		var _appLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

		_host = Host.CreateDefaultBuilder(e.Args)
			.UseSerilog((host, loggerConfiguration) =>
			{
				loggerConfiguration.WriteTo.File("tester.log", rollingInterval: RollingInterval.Day)
				.WriteTo.Debug()
				.MinimumLevel.Error()
				.MinimumLevel.Override("WhiteLabel.STDF", Serilog.Events.LogEventLevel.Debug);
			})
			.ConfigureAppConfiguration(x =>
			{
				x.SetBasePath(_appLocation);
			})
			.ConfigureServices(ConfigureServices)
			.Build();

		await _host.StartAsync();

		// Start the Main Window
		var _mainWindow = _host.Services.GetRequiredService<MainWindow>();
		_mainWindow.Show();
	}

	private void ConfigureServices(HostBuilderContext context, IServiceCollection services)
	{
		services.AddAutoMapper(Assembly.GetExecutingAssembly());
		services.AddTransient<MainWindow>();
	}

	private async void OnExit(object sender, ExitEventArgs e)
	{
		await _host.StopAsync();
		_host.Dispose();
		_host = null;
	}

	private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) { }
}
