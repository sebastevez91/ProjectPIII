using AutoPartesRazor.Data;
using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Products;

[Authorize]
public class CreateReviewModel : PageModel
{
    private readonly AutoPartesRazorContext _context;

    public CreateReviewModel(AutoPartesRazorContext context)
    {
        _context = context;
    }

    public Product Product { get; set; } = default!;
    public int CartCount { get; set; }

    [BindProperty]
    public ProductReview Review { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int productId)
    {
        Product = await _context.Product
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.id == productId);

        if (Product == null)
            return NotFound();

        // Verificar si el usuario ya reseñó este producto
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
        if (user != null)
        {
            var existingReview = await _context.ProductReview
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

            if (existingReview != null)
            {
                TempData["ErrorMessage"] = "Ya has escrito una reseña para este producto.";
                return RedirectToPage("/Products/Reviews", new { productId });
            }
        }

        CartCount = await _context.Cart.CountAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int productId)
    {
        Product = await _context.Product.FindAsync(productId);
        if (Product == null)
            return NotFound();

        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
        if (user == null)
            return RedirectToPage("/Account/Login");

        // Verificar duplicados
        var existingReview = await _context.ProductReview
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

        if (existingReview != null)
        {
            TempData["ErrorMessage"] = "Ya has escrito una reseña para este producto.";
            return RedirectToPage("/Products/Reviews", new { productId });
        }

        if (!ModelState.IsValid)
        {
            CartCount = await _context.Cart.CountAsync();
            return Page();
        }

        Review.ProductId = productId;
        Review.UserId = user.Id;
        Review.CreatedAt = DateTime.Now;

        _context.ProductReview.Add(Review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "¡Gracias por tu reseña!";
        return RedirectToPage("/Products/Reviews", new { productId });
    }
}