using AutoPartesRazor.Data;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Services;

/// Implementación del servicio de gestión de reclamos
public class ClaimService : IClaimService
{
    private readonly AutoPartesRazorContext _context;

    public ClaimService(AutoPartesRazorContext context)
    {
        _context = context;
    }

    // ========== OPERACIONES DE RECLAMO ==========

    public async Task<List<Claim>> ObtenerTodosLosReclamosAsync()
    {
        return await _context.Claims
            .Include(r => r.Cliente)
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
            .OrderByDescending(r => r.NivelUrgencia)
            .ThenByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Claim>> ObtenerReclamosPorClienteAsync(string clienteId)
    {
        return await _context.Claims
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
            .Where(r => r.ClienteId == clienteId)
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<Claim?> ObtenerReclamoPorIdAsync(int id)
    {
        return await _context.Claims
            .Include(r => r.Cliente)
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
                .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Claim?> ObtenerReclamoPorNumeroTicketAsync(string numeroTicket)
    {
        return await _context.Claims
            .Include(r => r.Cliente)
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
                .ThenInclude(m => m.Usuario)
            .FirstOrDefaultAsync(r => r.NumeroTicket == numeroTicket);
    }

    public async Task<Claim> CrearReclamoAsync(string clienteId, string asunto, string descripcion, LevelUrgency nivelUrgencia)
    {
        var numeroTicket = await GenerarNumeroTicketAsync();

        var reclamo = new Claim
        {
            NumeroTicket = numeroTicket,
            ClienteId = clienteId,
            Asunto = asunto,
            Descripcion = descripcion,
            NivelUrgencia = nivelUrgencia,
            Estado = StatusClaim.Nuevo,
            FechaCreacion = DateTime.Now,
            FechaActualizacion = DateTime.Now
        };

        _context.Claims.Add(reclamo);
        await _context.SaveChangesAsync();

        // Agregar el mensaje inicial del cliente con la descripción
        await AgregarMensajeAsync(reclamo.Id, clienteId, descripcion, false);

        return reclamo;
    }

    public async Task<bool> ActualizarEstadoReclamoAsync(int reclamoId, StatusClaim nuevoEstado)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null) return false;

        reclamo.Estado = nuevoEstado;
        reclamo.FechaActualizacion = DateTime.Now;

        if (nuevoEstado == StatusClaim.Cerrado)
        {
            reclamo.FechaCierre = DateTime.Now;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActualizarUrgenciaReclamoAsync(int reclamoId, LevelUrgency nuevaUrgencia)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null) return false;

        reclamo.NivelUrgencia = nuevaUrgencia;
        reclamo.FechaActualizacion = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AsignarAdministradorAsync(int reclamoId, string administradorId)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null) return false;

        reclamo.AdministradorAsignadoId = administradorId;
        reclamo.FechaActualizacion = DateTime.Now;

        // Si el reclamo está en estado Nuevo, cambiar a EnProceso
        if (reclamo.Estado == StatusClaim.Nuevo)
        {
            reclamo.Estado = StatusClaim.EnProceso;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CerrarReclamoAsync(int reclamoId)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null) return false;

        // Solo se puede cerrar si está en estado Resuelto
        if (reclamo.Estado != StatusClaim.Resuelto)
        {
            return false;
        }

        reclamo.Estado = StatusClaim.Cerrado;
        reclamo.FechaCierre = DateTime.Now;
        reclamo.FechaActualizacion = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerarNumeroTicketAsync()
    {
        var año = DateTime.Now.Year;
        var ultimoReclamo = await _context.Claims
            .Where(r => r.NumeroTicket.StartsWith($"RCL-{año}-"))
            .OrderByDescending(r => r.NumeroTicket)
            .FirstOrDefaultAsync();

        int siguienteNumero = 1;

        if (ultimoReclamo != null)
        {
            var partes = ultimoReclamo.NumeroTicket.Split('-');
            if (partes.Length == 3 && int.TryParse(partes[2], out int numero))
            {
                siguienteNumero = numero + 1;
            }
        }

        return $"RCL-{año}-{siguienteNumero:D5}";
    }

    public async Task<List<Claim>> ObtenerReclamosPorEstadoAsync(StatusClaim estado)
    {
        return await _context.Claims
            .Include(r => r.Cliente)
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
            .Where(r => r.Estado == estado)
            .OrderByDescending(r => r.NivelUrgencia)
            .ThenByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<List<Claim>> ObtenerReclamosPorUrgenciaAsync(LevelUrgency urgencia)
    {
        return await _context.Claims
            .Include(r => r.Cliente)
            .Include(r => r.AdministradorAsignado)
            .Include(r => r.Mensajes)
            .Where(r => r.NivelUrgencia == urgencia)
            .OrderByDescending(r => r.FechaCreacion)
            .ToListAsync();
    }

    public async Task<ReclamoEstadisticas> ObtenerEstadisticasAsync()
    {
        var reclamos = await _context.Claims.Include(r => r.Mensajes).ToListAsync();

        var estadisticas = new ReclamoEstadisticas
        {
            TotalReclamos = reclamos.Count,
            ReclamosNuevos = reclamos.Count(r => r.Estado == StatusClaim.Nuevo),
            ReclamosEnProceso = reclamos.Count(r => r.Estado == StatusClaim.EnProceso),
            ReclamosResueltos = reclamos.Count(r => r.Estado == StatusClaim.Resuelto),
            ReclamosCerrados = reclamos.Count(r => r.Estado == StatusClaim.Cerrado),
            ReclamosCriticos = reclamos.Count(r => r.NivelUrgencia == LevelUrgency.Critica && r.Estado != StatusClaim.Cerrado),
            ReclamosSinAsignar = reclamos.Count(r => r.AdministradorAsignadoId == null && r.Estado != StatusClaim.Cerrado)
        };

        // Calcular tiempo promedio de primera respuesta
        var reclamosConRespuesta = reclamos
            .Where(r => r.Mensajes.Any(m => m.EsAdministrador))
            .ToList();

        if (reclamosConRespuesta.Any())
        {
            var tiemposRespuesta = reclamosConRespuesta.Select(r =>
            {
                var primerMensajeAdmin = r.Mensajes
                    .Where(m => m.EsAdministrador)
                    .OrderBy(m => m.FechaEnvio)
                    .FirstOrDefault();

                if (primerMensajeAdmin != null)
                {
                    return (primerMensajeAdmin.FechaEnvio - r.FechaCreacion).TotalHours;
                }
                return 0;
            }).Where(t => t > 0);

            if (tiemposRespuesta.Any())
            {
                estadisticas.TiempoPromedioRespuestaHoras = tiemposRespuesta.Average();
            }
        }

        return estadisticas;
    }

    // ========== OPERACIONES DE MENSAJES ==========

    public async Task<MessageClaim> AgregarMensajeAsync(int reclamoId, string usuarioId, string mensaje, bool esAdministrador)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null)
        {
            throw new InvalidOperationException("El reclamo no existe.");
        }

        var nuevoMensaje = new MessageClaim
        {
            ReclamoId = reclamoId,
            UsuarioId = usuarioId,
            Mensaje = mensaje,
            FechaEnvio = DateTime.Now,
            EsAdministrador = esAdministrador,
            Leido = false
        };

        _context.MensajeReclamo.Add(nuevoMensaje);

        // Actualizar el estado del reclamo
        reclamo.FechaActualizacion = DateTime.Now;

        if (esAdministrador)
        {
            // Si es un administrador respondiendo, cambiar estado a Respondido
            if (reclamo.Estado == StatusClaim.Nuevo || reclamo.Estado == StatusClaim.EnProceso)
            {
                reclamo.Estado = StatusClaim.Respondido;
            }

            // Asignar automáticamente al administrador si no está asignado
            if (string.IsNullOrEmpty(reclamo.AdministradorAsignadoId))
            {
                reclamo.AdministradorAsignadoId = usuarioId;
            }
        }

        await _context.SaveChangesAsync();
        return nuevoMensaje;
    }

    public async Task<List<MessageClaim>> ObtenerMensajesDeReclamoAsync(int reclamoId)
    {
        return await _context.MensajeReclamo
            .Include(m => m.Usuario)
            .Where(m => m.ReclamoId == reclamoId)
            .OrderBy(m => m.FechaEnvio)
            .ToListAsync();
    }

    public async Task<bool> MarcarMensajesComoLeidosAsync(int reclamoId, bool esPorAdministrador)
    {
        var mensajes = await _context.MensajeReclamo
            .Where(m => m.ReclamoId == reclamoId && !m.Leido)
            .ToListAsync();

        var mensajesPorMarcar = esPorAdministrador
            ? mensajes.Where(m => !m.EsAdministrador).ToList()
            : mensajes.Where(m => m.EsAdministrador).ToList();

        foreach (var mensaje in mensajesPorMarcar)
        {
            mensaje.Leido = true;
            mensaje.FechaLectura = DateTime.Now;
        }

        if (mensajesPorMarcar.Any())
        {
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> UsuarioPuedeVerReclamoAsync(int reclamoId, string usuarioId, bool esAdministrador)
    {
        var reclamo = await _context.Claims.FindAsync(reclamoId);
        if (reclamo == null) return false;

        // Los administradores pueden ver todos los reclamos
        if (esAdministrador) return true;

        // Los clientes solo pueden ver sus propios reclamos
        return reclamo.ClienteId == usuarioId;
    }
}
