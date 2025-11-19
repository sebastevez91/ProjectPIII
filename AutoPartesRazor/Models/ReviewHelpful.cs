namespace AutoPartesRazor.Models;

/// Registro de votos útil/no útil por usuario (un voto por usuario por reseña)
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