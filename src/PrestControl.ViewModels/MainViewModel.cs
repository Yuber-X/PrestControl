using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrestControl.Common;
using Serilog;

namespace PrestControl.ViewModels;

/// <summary>Página de destino de la navegación del sidebar.</summary>
public enum Pagina
{
    Panel,
    Clientes,
    Prestamos,
    NuevoPrestamo,
    Cobros,
    Reportes,
    Historial,
    Configuracion
}

/// <summary>
/// Shell principal: página activa + cableado de la navegación entre módulos
/// (lista → detalle → cobros). Las páginas sin implementar muestran un placeholder.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly PrestamosViewModel _prestamosVm;
    private readonly PrestamoNuevoViewModel _nuevoVm;
    private readonly PrestamoDetalleViewModel _detalleVm;
    private readonly CobrosViewModel _cobrosVm;

    public MainViewModel(PrestamosViewModel prestamosVm, PrestamoNuevoViewModel nuevoVm,
        PrestamoDetalleViewModel detalleVm, CobrosViewModel cobrosVm)
    {
        _prestamosVm = prestamosVm;
        _nuevoVm = nuevoVm;
        _detalleVm = detalleVm;
        _cobrosVm = cobrosVm;

        // Navegación entre módulos disparada por los propios ViewModels
        _prestamosVm.NuevoSolicitado += () => _ = NavegarAsync(Pagina.NuevoPrestamo);
        _prestamosVm.DetalleSolicitado += id => _ = AbrirDetalleAsync(id);
        _nuevoVm.PrestamoCreado += id => _ = AbrirDetalleAsync(id);
        _detalleVm.VolverSolicitado += () => _ = NavegarAsync(Pagina.Prestamos);
        _detalleVm.CobrarSolicitado += id => _ = AbrirCobrosAsync(id);
    }

    [ObservableProperty]
    private Pagina _paginaActual = Pagina.Panel;

    [ObservableProperty]
    private object? _paginaActualVm;

    [ObservableProperty]
    private string _tituloPagina = "Panel de control";

    public string NombreUsuario => SesionActual.Nombre;
    public string Iniciales
    {
        get
        {
            var partes = SesionActual.Nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length switch
            {
                0 => "?",
                1 => partes[0][..1].ToUpperInvariant(),
                _ => $"{partes[0][..1]}{partes[1][..1]}".ToUpperInvariant()
            };
        }
    }

    /// <summary>Carga inicial del shell (llamada al abrir la ventana principal).</summary>
    public Task InicializarAsync() => NavegarAsync(Pagina.Panel);

    [RelayCommand]
    private async Task NavegarAsync(Pagina destino)
    {
        try
        {
            PaginaActual = destino;
            TituloPagina = TituloDe(destino);

            switch (destino)
            {
                case Pagina.Prestamos:
                    await _prestamosVm.CargarAsync();
                    PaginaActualVm = _prestamosVm;
                    break;
                case Pagina.NuevoPrestamo:
                    await _nuevoVm.CargarAsync();
                    PaginaActualVm = _nuevoVm;
                    break;
                case Pagina.Cobros:
                    await _cobrosVm.CargarAsync();
                    PaginaActualVm = _cobrosVm;
                    break;
                default:
                    PaginaActualVm = new PlaceholderViewModel(TituloDe(destino));
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error navegando a {Destino}", destino);
        }
    }

    private async Task AbrirDetalleAsync(long prestamoId)
    {
        try
        {
            PaginaActual = Pagina.Prestamos;
            TituloPagina = "Detalle de préstamo";
            await _detalleVm.CargarAsync(prestamoId);
            PaginaActualVm = _detalleVm;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error abriendo el detalle del préstamo {Id}", prestamoId);
        }
    }

    private async Task AbrirCobrosAsync(long prestamoId)
    {
        try
        {
            PaginaActual = Pagina.Cobros;
            TituloPagina = TituloDe(Pagina.Cobros);
            await _cobrosVm.CargarAsync(prestamoId);
            PaginaActualVm = _cobrosVm;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error abriendo cobros para el préstamo {Id}", prestamoId);
        }
    }

    private static string TituloDe(Pagina pagina) => pagina switch
    {
        Pagina.Panel => "Panel de control",
        Pagina.Clientes => "Clientes",
        Pagina.Prestamos => "Préstamos",
        Pagina.NuevoPrestamo => "Nuevo préstamo",
        Pagina.Cobros => "Cobros",
        Pagina.Reportes => "Reportes",
        Pagina.Historial => "Historial",
        Pagina.Configuracion => "Configuración",
        _ => string.Empty
    };
}
