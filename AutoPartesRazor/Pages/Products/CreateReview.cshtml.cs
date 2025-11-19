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
<<<<<<< HEAD
        Product = await _context.Products
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == productId);
=======
        Product = await _context.Product
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.id == productId);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090

        if (Product == null)
            return NotFound();

<<<<<<< HEAD
        // Verificar si el usuario ya rese?? este producto
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
        if (user != null)
        {
            var existingReview = await _context.ProductReviews
=======
        // Verificar si el usuario ya reseñó este producto
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
        if (user != null)
        {
            var existingReview = await _context.ProductReview
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

            if (existingReview != null)
            {
<<<<<<< HEAD
                TempData["ErrorMessage"] = "Ya has escrito una rese?a para este producto.";
=======
                TempData["ErrorMessage"] = "Ya has escrito una reseña para este producto.";
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
                return RedirectToPage("/Products/Reviews", new { productId });
            }
        }

<<<<<<< HEAD
        CartCount = await _context.Carts.CountAsync();
=======
        CartCount = await _context.Cart.CountAsync();
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int productId)
    {
<<<<<<< HEAD
        Product = await _context.Products.FindAsync(productId);
        if (Product == null)
            return NotFound();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
=======
        Product = await _context.Product.FindAsync(productId);
        if (Product == null)
            return NotFound();

        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == User.Identity!.Name);
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (user == null)
            return RedirectToPage("/Account/Login");

        // Verificar duplicados
<<<<<<< HEAD
        var existingReview = await _context.ProductReviews
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

        Review.ProductId = productId;
        Review.UserId = user.Id;
        Review.CreatedAt = DateTime.Now;

=======
        var existingReview = await _context.ProductReview
            .FirstOrDefaultAsync(r => r.ProductId == productId && r.UserId == user.Id);

>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        if (existingReview != null)
        {
            TempData["ErrorMessage"] = "Ya has escrito una reseña para este producto.";
            return RedirectToPage("/Products/Reviews", new { productId });
        }

        if (!ModelState.IsValid)
        {
<<<<<<< HEAD
            CartCount = await _context.Carts.CountAsync();
            return Page();
        }

        _context.ProductReviews.Add(Review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Gracias por tu reseña!";
=======
            CartCount = await _context.Cart.CountAsync();
            return Page();
        }

        Review.ProductId = productId;
        Review.UserId = user.Id;
        Review.CreatedAt = DateTime.Now;

        _context.ProductReview.Add(Review);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "¡Gracias por tu reseña!";
>>>>>>> c21700ccb191ba5352ac20e138b4c311c4f8d090
        return RedirectToPage("/Products/Reviews", new { productId });
    }
}