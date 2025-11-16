using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoPartesRazor.Models
{
    public class MessageClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "ID del Reclamo")]
        public int ReclamoId { get; set; }

        [ForeignKey(nameof(ReclamoId))]
        [Display(Name = "Reclamo")]
        public virtual Claim? Reclamo { get; set; }

        [Required]
        [Display(Name = "ID del Usuario")]
        public string UsuarioId { get; set; } = string.Empty;

        [ForeignKey(nameof(UsuarioId))]
        [Display(Name = "Usuario")]
        public virtual User? Usuario { get; set; }

        [Required(ErrorMessage = "El mensaje no puede estar vacío.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "El mensaje debe tener entre 1 y 2000 caracteres.")]
        [Display(Name = "Mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fecha de Envío")]
        public DateTime FechaEnvio { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Es Administrador")]
        public bool EsAdministrador { get; set; } = false;

        [Required]
        [Display(Name = "Leído")]
        public bool Leido { get; set; } = false;

        [Display(Name = "Fecha de Lectura")]
        public DateTime? FechaLectura { get; set; }
    }
}
