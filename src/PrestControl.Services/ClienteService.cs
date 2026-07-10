using PrestControl.Data;
using PrestControl.Models;

namespace PrestControl.Services;

/// <summary>
/// Lectura de clientes para los módulos de préstamos y cobros.
/// El CRUD completo (crear/editar/eliminar con auditoría) llega en Fase 2.
/// </summary>
public class ClienteService
{
    private readonly ClienteRepository _clientes;

    public ClienteService(ClienteRepository clientes) => _clientes = clientes;

    public Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default) =>
        _clientes.ObtenerActivosAsync(ct);

    public Task<Cliente?> ObtenerPorIdAsync(long id, CancellationToken ct = default) =>
        _clientes.ObtenerPorIdAsync(id, ct);
}
