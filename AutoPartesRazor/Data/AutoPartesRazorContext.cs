using AutoPartesRazor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Data;

public class AutoPartesRazorContext : IdentityDbContext<User>
{
    public AutoPartesRazorContext(DbContextOptions<AutoPartesRazorContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)        // Un producto tiene 1 marca
            .WithMany(b => b.products)      // Una marca tiene muchos productos
            .HasForeignKey(b => b.idBrand)
            .OnDelete(DeleteBehavior.Cascade); // Si borrámos marca, borra sus productos

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)        // Un producto tiene 1 categoria
            .WithMany(c => c.products)      // Una categoria tiene muchos productos
            .HasForeignKey(p => p.idCategory)
            .OnDelete(DeleteBehavior.Cascade); // Si borrámos la categoria, borra sus productos
    }

    public DbSet<AutoPartesRazor.Models.Product> Product { get; set; } = default!;

    public DbSet<AutoPartesRazor.Models.Brand> Brand { get; set; }

    public DbSet<AutoPartesRazor.Models.Category> Category { get; set; }

    public DbSet<AutoPartesRazor.Models.User> User { get; set; }

    public DbSet<AutoPartesRazor.Models.Cart> Cart { get; set; }

    // Nuevos DbSet para órdenes
    public DbSet<AutoPartesRazor.Models.Order> Order { get; set; }
    public DbSet<AutoPartesRazor.Models.OrderItem> OrderItem { get; set; }
}
