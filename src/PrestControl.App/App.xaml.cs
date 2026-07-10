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
            var ajustes = _servicios.GetRequiredService<PrestControl.Common.AjustesLocales>();

            // Tamaño de texto guardado + reacción a cambios desde Configuración
            shell.AplicarEscala(ajustes.FactorEscala);
            _servicios.GetRequiredService<ConfiguracionViewModel>().EscalaCambiada += shell.AplicarEscala;

            MainWindow = shell;
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            shell.Show();
            login.Close();
            _ = _servicios.GetRequiredService<MainViewModel>().InicializarAsync();

            // Export automático a Excel (si está activo y toca) — en segundo plano
            _ = _servicios.GetRequiredService<ExportacionService>().EjecutarAutomaticoSiTocaAsync(ajustes);

            // Aviso de clientes pasados de fecha (una vez por arranque + cambio de día)
            _servicios.GetRequiredService<NotificadorVencidos>().Iniciar();
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
        servicios.AddSingleton<ClienteRepository>();
        servicios.AddSingleton<PrestamoRepository>();
        servicios.AddSingleton<PagoRepository>();
        servicios.AddSingleton<ContadorRepository>();
        servicios.AddSingleton<DashboardRepository>();
        servicios.AddSingleton<ReporteRepository>();
        servicios.AddSingleton<ExportacionRepository>();

        // Services
        servicios.AddSingleton<AuditoriaService>();
        servicios.AddSingleton<AuthService>();
        servicios.AddSingleton<AmortizacionService>();
        servicios.AddSingleton<ClienteService>();
        servicios.AddSingleton<PrestamoService>();
        servicios.AddSingleton<PagoService>();
        servicios.AddSingleton<DashboardService>();
        servicios.AddSingleton<ReporteService>();
        servicios.AddSingleton<ExportacionService>();
        servicios.AddSingleton(sp =>
            new RespaldoService(sp.GetRequiredService<ConexionFactory>().CadenaConexion));
        servicios.AddSingleton(PrestControl.Common.AjustesLocales.Cargar());
        servicios.AddSingleton<PrestControl.Common.IDialogService, DialogService>();
        servicios.AddSingleton<NotificadorVencidos>();

        // ViewModels
        servicios.AddSingleton<LoginViewModel>();
        servicios.AddSingleton<ClientesViewModel>();
        servicios.AddSingleton<ClienteFichaViewModel>();
        servicios.AddSingleton<ClienteFormViewModel>();
        servicios.AddSingleton<PrestamosViewModel>();
        servicios.AddSingleton<PrestamoNuevoViewModel>();
        servicios.AddSingleton<PrestamoDetalleViewModel>();
        servicios.AddSingleton<CobrosViewModel>();
        servicios.AddSingleton<PanelViewModel>();
        servicios.AddSingleton<ReportesViewModel>();
        servicios.AddSingleton<HistorialViewModel>();
        servicios.AddSingleton<ConfiguracionViewModel>();
        servicios.AddSingleton<MainViewModel>();

        // Views
        servicios.AddSingleton<LoginWindow>();
        servicios.AddSingleton<MainWindow>();

        return servicios.BuildServiceProvider();
    }
}
