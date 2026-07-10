using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrestControl.Common;

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
/// Shell principal: estado del sidebar y página activa.
/// Las páginas reales se implementan por fases; mientras tanto el
/// ContentControl muestra un placeholder con el título del módulo.
/// </summary>
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private Pagina _paginaActual = Pagina.Panel;

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

    [RelayCommand]
    private void Navegar(Pagina destino)
    {
        PaginaActual = destino;
        TituloPagina = destino switch
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
}
