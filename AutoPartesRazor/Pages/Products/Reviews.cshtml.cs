using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

public class ReviewsModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public ReviewsModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Product Product { get; set; } = default!;
    public List<ProductReview> Reviews { get; set; } = new();
    public List<ReviewHelpful> UserVotes { get; set; } = new();
    public bool UserHasReviewed { get; set; }
    public int CartCount { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FilterRating { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "recent";

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        Product = await _context.Products
            .Include(p => p.Brand)
            .Include(p => p.Reviews!)
                .ThenInclude(r => r.User)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (Product == null)
            return NotFound();

        // Obtener rese?as con filtros
        var query = _context.ProductReviews
            .Include(r => r.User)
            .Where(r => r.ProductId == productId);

        if (FilterRating.HasValue)
            query = query.Where(r => r.Rating == FilterRating.Value);

        // Ordenamiento
        query = SortBy switch
        {
            "helpful" => query.OrderByDescending(r => r.HelpfulCount),
            "highest" => query.OrderByDescending(r => r.Rating),
            "lowest" => query.OrderBy(r => r.Rating),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        Reviews = await query.ToListAsync();

        // Verificar si el usuario ya rese??
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (user != null)
            {
                UserHasReviewed = await _context.ProductReviews
                    .AnyAsync(r => r.ProductId == productId && r.UserId == user.Id);

                // Obtener votos del usuario
                UserVotes = await _context.ReviewHelpfuls
                    .Where(v => v.UserId == user.Id && Reviews.Select(r => r.Id).Contains(v.ReviewId))
                    .ToListAsync();
            }
        }

        CartCount = await _context.Carts.CountAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostVoteAsync(int reviewId, bool isHelpful, int productId)
    {
        if (!User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Account/Login");

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity.Name);
        if (user == null)
            return RedirectToPage("/Account/Login");

        var review = await _context.ProductReviews.FindAsync(reviewId);
        if (review == null)
            return NotFound();

        // Verificar si ya vot?
        var existingVote = await _context.ReviewHelpfuls
            .FirstOrDefaultAsync(v => v.ReviewId == reviewId && v.UserId == user.Id);

        if (existingVote != null)
        {
            TempData["ErrorMessage"] = "Ya has votado en esta rese?a.";
            return RedirectToPage(new { productId });
        }

        // Crear voto
        var vote = new ReviewHelpful
        {
            ReviewId = reviewId,
            UserId = user.Id,
            IsHelpful = isHelpful,
            VotedAt = DateTime.Now
        };

        _context.ReviewHelpfuls.Add(vote);

        // Actualizar contadores
        if (isHelpful)
            review.HelpfulCount++;
        else
            review.NotHelpfulCount++;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "?Gracias por tu voto!";
        return RedirectToPage(new { productId });
    }
}