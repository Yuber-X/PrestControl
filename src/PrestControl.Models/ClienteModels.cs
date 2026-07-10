namespace PrestControl.Models;

/// <summary>Fila de la lista de clientes con agregados de sus préstamos (una sola consulta).</summary>
public record ClienteResumen(
    long Id,
    string Cedula,
    string Nombre,
    string Apellido,
    string? Telefono,
    int PrestamosActivos,
    decimal SaldoPendiente)
{
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();
}

/// <summary>Métricas de la ficha de cliente (mockup 3: cinco cards resumen).</summary>
public record ClienteMetricas(
    decimal TotalPrestado,
    decimal TotalCobrado,
    decimal SaldoPendiente,
    int PrestamosActivos,
    int CuotasVencidas);

/// <summary>Datos que captura el formulario de cliente (nuevo o edición).</summary>
public record ClienteDatos(
    string Cedula,
    string Nombre,
    string Apellido,
    string? Telefono,
    string? Direccion,
    string? Email,
    string? Notas);
