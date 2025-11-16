using AutoPartesRazor.Models;
using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.ViewModels
{
    /// ViewModel para crear un nuevo reclamo
    public class CrearReclamoViewModel
    {
        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "El asunto debe tener entre 10 y 200 caracteres.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "La descripción debe tener entre 20 y 2000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Nivel de Urgencia Sugerido")]
        public LevelUrgency NivelUrgencia { get; set; } = LevelUrgency.Media;
    }

    /// <summary>
    /// ViewModel para mostrar detalles de un reclamo
    /// </summary>
    public class ReclamoDetalleViewModel
    {
        public Claim Reclamo { get; set; } = new Claim();
        public List<MessageClaim> Mensajes { get; set; } = new List<MessageClaim>();

        [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "El mensaje debe tener entre 1 y 2000 caracteres.")]
        [Display(Name = "Nuevo Mensaje")]
        public string NuevoMensaje { get; set; } = string.Empty;

        public bool EsAdministrador { get; set; }
        public bool PuedeResponder { get; set; }
    }

   
    /// ViewModel para gestionar un reclamo (administrador)

    public class GestionarReclamoViewModel
    {
        public Claim Reclamo { get; set; } = new Claim();
        public List<MessageClaim> Mensajes { get; set; } = new List<MessageClaim>();

        [Display(Name = "Estado")]
        public StatusClaim NuevoEstado { get; set; }

        [Display(Name = "Nivel de Urgencia")]
        public LevelUrgency NuevaUrgencia { get; set; }

        [Display(Name = "Asignar a")]
        public string? AdministradorAsignadoId { get; set; }

        [StringLength(2000, MinimumLength = 1, ErrorMessage = "El mensaje debe tener entre 1 y 2000 caracteres.")]
        [Display(Name = "Respuesta")]
        public string? Respuesta { get; set; }

        public List<User> AdministradoresDisponibles { get; set; } = new List<User>();
    }

    /// ViewModel para filtros de reclamos
    public class FiltroReclamosViewModel
    {
        [Display(Name = "Estado")]
        public StatusClaim? Estado { get; set; }

        [Display(Name = "Urgencia")]
        public LevelUrgency? Urgencia { get; set; }

        [Display(Name = "Fecha Desde")]
        [DataType(DataType.Date)]
        public DateTime? FechaDesde { get; set; }

        [Display(Name = "Fecha Hasta")]
        [DataType(DataType.Date)]
        public DateTime? FechaHasta { get; set; }

        [Display(Name = "Buscar")]
        [StringLength(100)]
        public string? TextoBusqueda { get; set; }

        [Display(Name = "Administrador Asignado")]
        public string? AdministradorId { get; set; }

        [Display(Name = "Solo Sin Asignar")]
        public bool SoloSinAsignar { get; set; }
    }

}
