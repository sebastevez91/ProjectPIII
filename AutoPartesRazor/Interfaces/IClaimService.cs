using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;

namespace AutoPartesRazor.Interfaces;

/// Interfaz para el servicio de gestión de reclamos

public interface IClaimService
{
    // ========== OPERACIONES DE RECLAMO ==========

    /// Obtiene todos los reclamos del sistema
    Task<List<Claim>> ObtenerTodosLosReclamosAsync();

    /// Obtiene todos los reclamos de un cliente específico

    Task<List<Claim>> ObtenerReclamosPorClienteAsync(string clienteId);

    /// Obtiene un reclamo por su ID
    Task<Claim?> ObtenerReclamoPorIdAsync(int id);

    /// Obtiene un reclamo por su número de ticket
    Task<Claim?> ObtenerReclamoPorNumeroTicketAsync(string numeroTicket);

    /// Crea un nuevo reclamo
    Task<Claim> CrearReclamoAsync(string clienteId, string asunto, string descripcion, LevelUrgency nivelUrgencia);

    /// Actualiza el estado de un reclamo
    Task<bool> ActualizarEstadoReclamoAsync(int reclamoId, StatusClaim nuevoEstado);

    /// Actualiza el nivel de urgencia de un reclamo
    Task<bool> ActualizarUrgenciaReclamoAsync(int reclamoId, LevelUrgency nuevaUrgencia);

    /// Asigna un administrador a un reclamo
    Task<bool> AsignarAdministradorAsync(int reclamoId, string administradorId);

    /// Cierra un reclamo
    Task<bool> CerrarReclamoAsync(int reclamoId);

    /// Genera un número de ticket único
    Task<string> GenerarNumeroTicketAsync();

    /// Obtiene reclamos filtrados por estado
    Task<List<Claim>> ObtenerReclamosPorEstadoAsync(StatusClaim estado);

    /// Obtiene reclamos filtrados por urgencia
    Task<List<Claim>> ObtenerReclamosPorUrgenciaAsync(LevelUrgency urgencia);

    /// Obtiene estadísticas de reclamos
    Task<ReclamoEstadisticas> ObtenerEstadisticasAsync();

    // ========== OPERACIONES DE MENSAJES ==========

    /// Agrega un mensaje a un reclamo
    Task<MessageClaim> AgregarMensajeAsync(int reclamoId, string usuarioId, string mensaje, bool esAdministrador);

    /// Obtiene todos los mensajes de un reclamo
    Task<List<MessageClaim>> ObtenerMensajesDeReclamoAsync(int reclamoId);

    /// Marca los mensajes como leídos
    Task<bool> MarcarMensajesComoLeidosAsync(int reclamoId, bool esPorAdministrador);

    /// Verifica si el usuario tiene permisos para ver el reclamo
    Task<bool> UsuarioPuedeVerReclamoAsync(int reclamoId, string usuarioId, bool esAdministrador);
}

/// Clase para estadísticas de reclamos
public class ReclamoEstadisticas
{
    public int TotalReclamos { get; set; }
    public int ReclamosNuevos { get; set; }
    public int ReclamosEnProceso { get; set; }
    public int ReclamosResueltos { get; set; }
    public int ReclamosCerrados { get; set; }
    public int ReclamosCriticos { get; set; }
    public int ReclamosSinAsignar { get; set; }
    public double TiempoPromedioRespuestaHoras { get; set; }
}
