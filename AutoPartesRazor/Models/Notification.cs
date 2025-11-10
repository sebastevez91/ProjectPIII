namespace AutoPartesRazor.Models;

public class Notification
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsRead { get; set; } = false;
    // Foreign key to User
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}
