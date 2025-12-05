using AutoPartesRazor.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models
{
    public class Claim
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El número de ticket es obligatorio.")]
        [StringLength(50)]
        [Display(Name = "Número de Ticket")]
        public string NumeroTicket { get; set; } = string.Empty;

        [Required(ErrorMessage = "El asunto es obligatorio.")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "El asunto debe tener entre 10 y 200 caracteres.")]
        [Display(Name = "Asunto")]
        public string Asunto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, MinimumLength = 20, ErrorMessage = "La descripción debe tener entre 20 y 2000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nivel de Urgencia")]
        public LevelUrgency NivelUrgencia { get; set; } = LevelUrgency.Media;

        [Required]
        [Display(Name = "Estado")]
        public StatusClaim Estado { get; set; } = StatusClaim.Nuevo;

        [Required]
        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Última Actualización")]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Cierre")]
        public DateTime? FechaCierre { get; set; }

        // Relación con Cliente (Usuario que crea el reclamo)
        [Required]
        [Display(Name = "ID del Cliente")]
        public string ClienteId { get; set; } = string.Empty;

        [ForeignKey(nameof(ClienteId))]
        [Display(Name = "Cliente")]
        public virtual User? Cliente { get; set; }

        public int? OrderId { get; set; }  // Nullable porque puede ser un reclamo general
        public virtual Order? Order { get; set; }

        // Relación con Administrador asignado (opcional)
        [Display(Name = "ID del Administrador Asignado")]
        public string? AdministradorAsignadoId { get; set; }

        [ForeignKey(nameof(AdministradorAsignadoId))]
        [Display(Name = "Administrador Asignado")]
        public virtual User? AdministradorAsignado { get; set; }

        // AGREGAR ESTA PROPIEDAD
        public string? AreaAsignada { get; set; }

        // Colección de mensajes del reclamo
        public virtual ICollection<MessageClaim> Mensajes { get; set; } = new List<MessageClaim>();

        /// Indica si hay mensajes sin leer por parte del cliente
        [NotMapped]
        public bool TieneMensajesSinLeerCliente
        {
            get
            {
                return Mensajes?.Any(m => m.EsAdministrador && !m.Leido) ?? false;
            }
        }

        /// Indica si hay mensajes sin leer por parte del administrador
        [NotMapped]
        public bool TieneMensajesSinLeerAdmin
        {
            get
            {
                return Mensajes?.Any(m => !m.EsAdministrador && !m.Leido) ?? false;
            }
        }

        /// Contador de mensajes sin leer para el cliente
        [NotMapped]
        public int MensajesSinLeerCliente
        {
            get
            {
                return Mensajes?.Count(m => m.EsAdministrador && !m.Leido) ?? 0;
            }
        }

        /// Contador de mensajes sin leer para el administrador
        [NotMapped]
        public int MensajesSinLeerAdmin
        {
            get
            {
                return Mensajes?.Count(m => !m.EsAdministrador && !m.Leido) ?? 0;
            }
        }
    }
}