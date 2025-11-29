using AutoPartesRazor.Models;

namespace AutoPartesRazor.ViewModels;

/// <summary>
/// ViewModel para mostrar usuarios con reseñas negativas elegibles para cupones
/// </summary>
public class CouponFromReviewViewModel
{
    public int ReviewId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }

    public int SuggestedDiscount { get; set; }
    public bool HasExistingCoupon { get; set; }
    public string? ExistingCouponCode { get; set; }
}

/// <summary>
/// ViewModel para crear cupones masivos
/// </summary>
public class BulkCouponCreateViewModel
{
    public List<int> SelectedReviewIds { get; set; } = new();
    public int DaysValid { get; set; } = 30;
    public bool ApplyToSpecificProduct { get; set; } = true;
}