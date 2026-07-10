using MySqlConnector;
using PrestControl.Common;
using PrestControl.Models;

namespace PrestControl.Data;

/// <summary>
/// Lectura de clientes para selección en préstamos y cobros.
/// El CRUD completo (crear/editar/eliminar) llega en Fase 2.
/// Soft delete: toda lectura filtra deleted_at IS NULL.
/// </summary>
public class ClienteRepository
{
    private readonly ConexionFactory _factory;

    public ClienteRepository(ConexionFactory factory) => _factory = factory;

    public async Task<IReadOnlyList<Cliente>> ObtenerActivosAsync(CancellationToken ct = default)
    {
        using var conexion = await _factory.AbrirAsync(ct);
        using var cmd = conexion.CreateCommand();
        cmd.CommandText = $"""
            SELECT id, cedula, nombre, apellido, telefono, direccion, email, notas, created_at, updated_at
            FROM {DbNames.Cliente}
            WHERE deleted_at IS NULL
            ORDER BY nombre, apellido;
            """;

        var clientes = new List<Cliente>();
        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            clientes.Add(Mapear(reader));
        return clientes;
    }

    public async Task<Cliente?> ObtenerPorIdAsync(long id, CancellationToken ct = default)
    {
        using var conexion = await _factory.AbrirAsync(ct);
        using var cmd = conexion.CreateCommand();
        cmd.CommandText = $"""
            SELECT id, cedula, nombre, apellido, telefono, direccion, email, notas, created_at, updated_at
            FROM {DbNames.Cliente}
            WHERE id = @id AND deleted_at IS NULL;
            """;
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Mapear(reader) : null;
    }

    private static Cliente Mapear(MySqlDataReader reader) => new()
    {
        Id = reader.GetInt64("id"),
        Cedula = reader.GetString("cedula"),
        Nombre = reader.GetString("nombre"),
        Apellido = reader.GetString("apellido"),
        Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
        Direccion = reader.IsDBNull(reader.GetOrdinal("direccion")) ? null : reader.GetString("direccion"),
        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString("email"),
        Notas = reader.IsDBNull(reader.GetOrdinal("notas")) ? null : reader.GetString("notas"),
        CreatedAtUtc = DateTime.SpecifyKind(reader.GetDateTime("created_at"), DateTimeKind.Utc),
        UpdatedAtUtc = reader.IsDBNull(reader.GetOrdinal("updated_at"))
            ? null
            : DateTime.SpecifyKind(reader.GetDateTime("updated_at"), DateTimeKind.Utc)
    };
}
