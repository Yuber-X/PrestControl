using System.IO;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PrestControl.Data;
using PrestControl.Services;
using PrestControl.ViewModels;
using PrestControl.Views;
using Serilog;

namespace PrestControl.App;

/// <summary>
/// Bootstrap: Serilog + contenedor de dependencias + flujo login → shell.
/// ShutdownMode es OnExplicitShutdown porque cerramos la LoginWindow
/// antes de abrir el MainWindow.
/// </summary>
public partial class App : Application
{
    private ServiceProvider? _servicios;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ConfigurarSerilog();
        _servicios = ConfigurarServicios();

        Log.Information("PrestControl iniciando");

        var login = _servicios.GetRequiredService<LoginWindow>();
        var loginVm = _servicios.GetRequiredService<LoginViewModel>();

        loginVm.LoginExitoso += (_, _) =>
        {
            var shell = _servicios.GetRequiredService<MainWindow>();
            MainWindow = shell;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            shell.Show();
            login.Close();
        };

        login.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Cierre de sesión de respaldo si el usuario cerró la ventana sin logout
        try
        {
            var auth = _servicios?.GetService<AuthService>();
            auth?.LogoutAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "No se pudo registrar el logout al salir");
        }

        Log.Information("PrestControl finalizado");
        Log.CloseAndFlush();
        _servicios?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigurarSerilog()
    {
        var carpetaLogs = Path.Combine(AppContext.BaseDirectory, "logs");
        Directory.CreateDirectory(carpetaLogs);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(carpetaLogs, "prestcontrol-.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30)
            .CreateLogger();
    }

    private static ServiceProvider ConfigurarServicios()
    {
        var servicios = new ServiceCollection();

        // Data
        servicios.AddSingleton<ConexionFactory>();
        servicios.AddSingleton<UsuarioRepository>();
        servicios.AddSingleton<SesionRepository>();
        servicios.AddSingleton<AuditoriaRepository>();

        // Services
        servicios.AddSingleton<AuditoriaService>();
        servicios.AddSingleton<AuthService>();
        servicios.AddSingleton<AmortizacionService>();

        // ViewModels
        servicios.AddSingleton<LoginViewModel>();
        servicios.AddSingleton<MainViewModel>();

        // Views
        servicios.AddSingleton<LoginWindow>();
        servicios.AddSingleton<MainWindow>();

        return servicios.BuildServiceProvider();
    }
}
