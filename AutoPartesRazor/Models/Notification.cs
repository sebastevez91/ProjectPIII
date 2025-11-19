using System.ComponentModel.DataAnnotations;

namespace AutoPartesRazor.Models;

public class Notification
{
    public int Id { get; set; }
<<<<<<< HEAD

=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    [Required(ErrorMessage = "El asunto es requerido")]
    [StringLength(100, ErrorMessage = "El asunto no puede exceder los 100 caracteres")]
    [Display(Name = "Asunto")]
    public string Title { get; set; } = string.Empty;
<<<<<<< HEAD

=======
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
    [Required(ErrorMessage = "El mensaje es requerido")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "El mensaje debe tener entre 10 y 2000 caracteres")]
    [Display(Name = "Mensaje")]
    public string Message { get; set; } = string.Empty;
<<<<<<< HEAD

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsRead { get; set; } = false;

    // Foreign Key - User
    [Required]
    public string UserId { get; set; }

    // Foreign key to User
    public User? User { get; set; }
}
=======
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
    // Foreign key to User
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
