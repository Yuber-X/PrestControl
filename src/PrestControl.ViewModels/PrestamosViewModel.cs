using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrestControl.Common;
using PrestControl.Services;
using PrestControl.Models;
using Serilog;

namespace PrestControl.ViewModels;

/// <summary>Lista de préstamos con búsqueda por código o cliente.</summary>
public partial class PrestamosViewModel : ObservableObject
{
    private readonly PrestamoService _servicio;
    private readonly IDialogService _dialogos;
    private IReadOnlyList<PrestamoResumen> _todos = [];

    public event Action<long>? DetalleSolicitado;
    public event Action? NuevoSolicitado;

    public PrestamosViewModel(PrestamoService servicio, IDialogService dialogos)
    {
        _servicio = servicio;
        _dialogos = dialogos;
    }

    public ObservableCollection<PrestamoFila> Filas { get; } = [];

    [ObservableProperty]
    private string _textoBusqueda = string.Empty;

    [ObservableProperty]
    private bool _cargando;

    [ObservableProperty]
    private string _contadorTexto = string.Empty;

    partial void OnTextoBusquedaChanged(string value) => AplicarFiltro();

    public async Task CargarAsync()
    {
        try
        {
            Cargando = true;
            _todos = await _servicio.ObtenerResumenesAsync();
            AplicarFiltro();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error cargando la lista de préstamos");
            _dialogos.MostrarError("Préstamos", $"No se pudo cargar la lista de préstamos.\n\n{ex.Message}");
        }
        finally
        {
            Cargando = false;
        }
    }

    private void AplicarFiltro()
    {
        var filtro = TextoBusqueda.Trim();
        var visibles = string.IsNullOrEmpty(filtro)
            ? _todos
            : _todos.Where(p =>
                p.Codigo.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                p.ClienteNombre.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();

        Filas.Clear();
        foreach (var resumen in visibles)
            Filas.Add(new PrestamoFila(resumen));

        ContadorTexto = _todos.Count == 0
            ? "Sin préstamos registrados"
            : $"Mostrando {Filas.Count} de {_todos.Count} préstamos";
    }

    [RelayCommand]
    private void Nuevo() => NuevoSolicitado?.Invoke();

    [RelayCommand]
    private void VerDetalle(PrestamoFila? fila)
    {
        if (fila is not null)
            DetalleSolicitado?.Invoke(fila.Id);
    }
}
