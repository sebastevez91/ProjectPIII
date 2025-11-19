namespace AutoPartesRazor.Models;

<<<<<<< HEAD
/// Registro de votos útil/no útil por usuario (un voto por usuario por reseña)
=======
/// <summary>
/// Registro de votos útil/no útil por usuario (un voto por usuario por reseña)
/// </summary>
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
public class ReviewHelpful
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public ProductReview? Review { get; set; }

    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }

    public bool IsHelpful { get; set; } // true = útil, false = no útil
    public DateTime VotedAt { get; set; } = DateTime.Now;
}