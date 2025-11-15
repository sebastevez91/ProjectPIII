using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Notification
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El asunto es requerido")]
    [StringLength(100, ErrorMessage = "El asunto no puede exceder los 100 caracteres")]
    [Display(Name = "Asunto")]
    public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "El mensaje es requerido")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "El mensaje debe tener entre 10 y 2000 caracteres")]
    [Display(Name = "Mensaje")]
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
    // Foreign key to User
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}
