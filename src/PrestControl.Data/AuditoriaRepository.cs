using MySqlConnector;
using PrestControl.Common;
using PrestControl.Models;

namespace PrestControl.Data;

/// <summary>Escritura del log de auditoría. La tabla es inmutable: solo INSERT y SELECT.</summary>
public class AuditoriaRepository
{
    private readonly ConexionFactory _factory;

    public AuditoriaRepository(ConexionFactory factory) => _factory = factory;

    public async Task InsertarAsync(Auditoria entrada, CancellationToken ct = default)
    {
        using var conexion = await _factory.AbrirAsync(ct);
        await InsertarAsync(entrada, conexion, transaccion: null, ct);
    }

    /// <summary>
    /// Variante para participar en una transacción existente (venta de préstamo,
    /// registro de pago, etc. — la auditoría entra en la MISMA transacción atómica).
    /// </summary>
    public async Task InsertarAsync(Auditoria entrada, MySqlConnection conexion, MySqlTransaction? transaccion, CancellationToken ct = default)
    {
        using var cmd = conexion.CreateCommand();
        cmd.Transaction = transaccion;
        cmd.CommandText = $"""
            INSERT INTO {DbNames.Auditoria} (usuario_id, entidad, entidad_id, accion, descripcion, ip_local, timestamp)
            VALUES (@usuarioId, @entidad, @entidadId, @accion, @descripcion, @ipLocal, @timestamp);
            """;
        cmd.Parameters.AddWithValue("@usuarioId", entrada.UsuarioId);
        cmd.Parameters.AddWithValue("@entidad", entrada.Entidad);
        cmd.Parameters.AddWithValue("@entidadId", (object?)entrada.EntidadId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@accion", AccionADb(entrada.Accion));
        cmd.Parameters.AddWithValue("@descripcion", (object?)entrada.Descripcion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@ipLocal", (object?)entrada.IpLocal ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@timestamp", entrada.TimestampUtc);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    /// <summary>Mapea el enum C# al valor del ENUM MySQL.</summary>
    private static string AccionADb(AccionAuditoria accion) => accion switch
    {
        AccionAuditoria.Crear => "crear",
        AccionAuditoria.Modificar => "modificar",
        AccionAuditoria.Eliminar => "eliminar",
        AccionAuditoria.Consultar => "consultar",
        AccionAuditoria.Login => "login",
        AccionAuditoria.Logout => "logout",
        _ => throw new ArgumentOutOfRangeException(nameof(accion))
    };
}
