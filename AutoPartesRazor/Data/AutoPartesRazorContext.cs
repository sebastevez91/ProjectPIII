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

        // Relacion Notification - User
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ============================================
        // RELACIONES DE PROVEEDORES
        // ============================================

        modelBuilder.Entity<ProductSupplier>()
            .HasKey(ps => new { ps.ProductId, ps.SupplierId });

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSuppliers)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);

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

        // ============================================
        // NUEVAS RELACIONES DE RESEÑAS
        // ============================================

        // Relación Product - ProductReview
        modelBuilder.Entity<ProductReview>()
            .HasOne(pr => pr.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(pr => pr.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación User - ProductReview
        modelBuilder.Entity<ProductReview>()
            .HasOne(pr => pr.User)
            .WithMany()
            .HasForeignKey(pr => pr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación ReviewHelpful - ProductReview
        modelBuilder.Entity<ReviewHelpful>()
            .HasOne(rh => rh.Review)
            .WithMany()
            .HasForeignKey(rh => rh.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación ReviewHelpful - User
        modelBuilder.Entity<ReviewHelpful>()
            .HasOne(rh => rh.User)
            .WithMany()
            .HasForeignKey(rh => rh.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraint: Un usuario solo puede votar una vez por reseña
        modelBuilder.Entity<ReviewHelpful>()
            .HasIndex(rh => new { rh.ReviewId, rh.UserId })
            .IsUnique();

        // Constraint: Un usuario solo puede hacer una reseña por producto
        modelBuilder.Entity<ProductReview>()
            .HasIndex(pr => new { pr.ProductId, pr.UserId })
            .IsUnique();
    }

    public DbSet<AutoPartesRazor.Models.Product> Product { get; set; } = default!;
    public DbSet<AutoPartesRazor.Models.Brand> Brand { get; set; }
    public DbSet<AutoPartesRazor.Models.Category> Category { get; set; }
    public DbSet<AutoPartesRazor.Models.Cart> Cart { get; set; }
    public DbSet<AutoPartesRazor.Models.User> User { get; set; }
    public DbSet<AutoPartesRazor.Models.Order> Order { get; set; }
    public DbSet<AutoPartesRazor.Models.OrderItem> OrderItem { get; set; }
    public DbSet<AutoPartesRazor.Models.Notification> Notification { get; set; }
    public DbSet<AutoPartesRazor.Models.Supplier> Supplier { get; set; }
    public DbSet<AutoPartesRazor.Models.ProductSupplier> ProductSupplier { get; set; }
    public DbSet<AutoPartesRazor.Models.PurchaseOrder> PurchaseOrder { get; set; }

    // ============================================
    // NUEVAS TABLAS PARA RESEÑAS
    // ============================================
    public DbSet<AutoPartesRazor.Models.ProductReview> ProductReview { get; set; } = default!;
    public DbSet<AutoPartesRazor.Models.ReviewHelpful> ReviewHelpful { get; set; } = default!;

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