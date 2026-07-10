using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrestControl.Common;
using PrestControl.Models;
using PrestControl.Services;
using Serilog;

namespace PrestControl.ViewModels;

/// <summary>Fila de la tabla de clientes.</summary>
public record ClienteFila(ClienteResumen Resumen)
{
    public long Id => Resumen.Id;
    public string Cedula => Resumen.Cedula;
    public string NombreCompleto => Resumen.NombreCompleto;
    public string TelefonoTexto => string.IsNullOrWhiteSpace(Resumen.Telefono) ? "—" : Resumen.Telefono;
    public int PrestamosActivos => Resumen.PrestamosActivos;
    public decimal SaldoPendiente => Resumen.SaldoPendiente;
    public bool AlDia => Resumen.PrestamosActivos == 0 || Resumen.SaldoPendiente == 0m;
}

/// <summary>Lista de clientes con búsqueda por nombre, cédula o teléfono.</summary>
public partial class ClientesViewModel : ObservableObject
{
    private readonly ClienteService _servicio;
    private readonly IDialogService _dialogos;
    private IReadOnlyList<ClienteResumen> _todos = [];

    public event Action? NuevoSolicitado;
    public event Action<long>? FichaSolicitada;

    public ClientesViewModel(ClienteService servicio, IDialogService dialogos)
    {
        _servicio = servicio;
        _dialogos = dialogos;
    }

    public ObservableCollection<ClienteFila> Filas { get; } = [];

    [ObservableProperty]
    private string _textoBusqueda = string.Empty;

    [ObservableProperty]
    private string _contadorTexto = string.Empty;

    partial void OnTextoBusquedaChanged(string value) => AplicarFiltro();

    public async Task CargarAsync()
    {
        try
        {
            _todos = await _servicio.ObtenerResumenesAsync();
            AplicarFiltro();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error cargando la lista de clientes");
            _dialogos.MostrarError("Clientes", $"No se pudo cargar la lista de clientes.\n\n{ex.Message}");
        }
    }

    private void AplicarFiltro()
    {
        var filtro = TextoBusqueda.Trim();
        var visibles = string.IsNullOrEmpty(filtro)
            ? _todos
            : _todos.Where(c =>
                c.NombreCompleto.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                c.Cedula.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                (c.Telefono?.Contains(filtro, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

        Filas.Clear();
        foreach (var resumen in visibles)
            Filas.Add(new ClienteFila(resumen));

        ContadorTexto = _todos.Count == 0
            ? "Sin clientes registrados"
            : $"Mostrando {Filas.Count} de {_todos.Count} clientes";
    }

    [RelayCommand]
    private void Nuevo() => NuevoSolicitado?.Invoke();

    [RelayCommand]
    private void VerFicha(ClienteFila? fila)
    {
        if (fila is not null)
            FichaSolicitada?.Invoke(fila.Id);
    }
}
