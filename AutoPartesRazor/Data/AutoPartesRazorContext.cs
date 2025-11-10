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

        // Relación Product - Brand
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.products)
            .HasForeignKey(b => b.idBrand)
            .OnDelete(DeleteBehavior.Restrict); 

        // Relación Product - Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.products)
            .HasForeignKey(p => p.idCategory)
            .OnDelete(DeleteBehavior.Restrict); 

        // Relación Order - OrderItem
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================================
        // QUERY FILTERS - SOFT DELETE
        // ============================================
        // Estos filtros se aplican automáticamente a todas las consultas
        // Solo muestra registros donde IsDeleted = false

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<Brand>()
            .HasQueryFilter(b => !b.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => !o.IsDeleted);

        // ============================================
        // ÍNDICES PARA MEJORAR RENDIMIENTO
        // ============================================
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.IsDeleted);

        modelBuilder.Entity<Brand>()
            .HasIndex(b => b.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.IsDeleted);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.IsDeleted);
    }

    public DbSet<AutoPartesRazor.Models.Product> Product { get; set; } = default!;
    public DbSet<AutoPartesRazor.Models.Brand> Brand { get; set; }
    public DbSet<AutoPartesRazor.Models.Category> Category { get; set; }
    public DbSet<AutoPartesRazor.Models.Cart> Cart { get; set; }
    public DbSet<AutoPartesRazor.Models.User> User { get; set; }
    public DbSet<AutoPartesRazor.Models.Order> Order { get; set; }
    public DbSet<AutoPartesRazor.Models.OrderItem> OrderItem { get; set; }

    // ============================================
    // MÉTODO PARA GUARDAR CON SOFT DELETE AUTOMÁTICO
    // ============================================
    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void HandleSoftDelete()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            // Verificar si la entidad tiene la propiedad IsDeleted
            var isDeletedProperty = entry.Entity.GetType().GetProperty("IsDeleted");
            var deletedAtProperty = entry.Entity.GetType().GetProperty("DeletedAt");

            if (isDeletedProperty != null && deletedAtProperty != null)
            {
                // En lugar de eliminar, marcar como eliminado
                entry.State = EntityState.Modified;
                isDeletedProperty.SetValue(entry.Entity, true);
                deletedAtProperty.SetValue(entry.Entity, DateTime.Now);
            }
        }
    }
}