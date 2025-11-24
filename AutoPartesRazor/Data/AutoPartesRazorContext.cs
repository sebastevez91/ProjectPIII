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
            .WithMany(b => b.Products)
            .HasForeignKey(b => b.BrandId)
<<<<<<< HEAD
            .OnDelete(DeleteBehavior.Restrict); 
=======
            .OnDelete(DeleteBehavior.Restrict);

>>>>>>> main


        // Relación Product - Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
<<<<<<< HEAD
            .OnDelete(DeleteBehavior.Restrict); 
=======
            .OnDelete(DeleteBehavior.Restrict);

>>>>>>> main


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

        // Relación Order - User
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Cart - Product
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación PurchaseOrder - Product
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Product)
            .WithMany(p => p.PurchaseOrders)
            .HasForeignKey(po => po.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación PurchaseOrder - Supplier
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Supplier)
            .WithMany(s => s.PurchaseOrders)
            .HasForeignKey(po => po.SupplierId)
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

<<<<<<< HEAD
        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);
=======
        // Configurar StockMovement
        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.PurchaseOrder)
            .WithMany()
            .HasForeignKey(sm => sm.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockMovement>()
            .HasOne(sm => sm.StockAdjustment)
            .WithOne(sa => sa.StockMovement)
            .HasForeignKey<StockMovement>(sm => sm.StockAdjustmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar StockAdjustment
        modelBuilder.Entity<StockAdjustment>()
            .HasOne(sa => sa.Product)
            .WithMany(p => p.StockAdjustments)
            .HasForeignKey(sa => sa.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockAdjustment>()
            .HasOne(sa => sa.Supplier)
            .WithMany()
            .HasForeignKey(sa => sa.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockAdjustment>()
            .HasOne(sa => sa.RelatedPurchaseOrder)
            .WithMany()
            .HasForeignKey(sa => sa.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar SupplierClaim
        modelBuilder.Entity<SupplierClaim>()
            .HasOne(sc => sc.Supplier)
            .WithMany()
            .HasForeignKey(sc => sc.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierClaim>()
            .HasOne(sc => sc.PurchaseOrder)
            .WithMany()
            .HasForeignKey(sc => sc.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupplierClaim>()
            .HasOne(sc => sc.StockAdjustment)
            .WithOne(sa => sa.SupplierClaim)
            .HasForeignKey<SupplierClaim>(sc => sc.StockAdjustmentId)
            .OnDelete(DeleteBehavior.Restrict);
>>>>>>> main

        // ============================================
        // RELACIONES MÓDULO RECLAMOS
        // ============================================

        // Relación Reclamo - Cliente
        modelBuilder.Entity<Claim>()
            .HasOne(c => c.Cliente)
            .WithMany()
            .HasForeignKey(c => c.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Reclamo - Administrador Asignado
        modelBuilder.Entity<Claim>()
            .HasOne(c => c.AdministradorAsignado)
            .WithMany()
            .HasForeignKey(c => c.AdministradorAsignadoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación MensajeReclamo - Reclamo
        modelBuilder.Entity<MessageClaim>()
            .HasOne(m => m.Reclamo)
            .WithMany(r => r.Mensajes)
            .HasForeignKey(m => m.ReclamoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación MensajeReclamo - Usuario
        modelBuilder.Entity<MessageClaim>()
            .HasOne(m => m.Usuario)
            .WithMany()
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // ============================================
<<<<<<< HEAD
        // NUEVAS RELACIONES DE RESEÑAS
=======
        // RELACIONES DE RESEÑAS
>>>>>>> main
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

        // ============================================
<<<<<<< HEAD
=======
        // RELACIONES DE CUPONES
        // ============================================

        // Relación Coupon - User
        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Coupon - Product (opcional)
        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Coupon - ProductReview (opcional)
        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Review)
            .WithMany()
            .HasForeignKey(c => c.ReviewId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Coupon - Order (opcional)
        modelBuilder.Entity<Coupon>()
            .HasOne(c => c.Order)
            .WithMany()
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único para códigos de cupón
        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.Code)
            .IsUnique();

        // Índices para mejorar rendimiento de cupones
        modelBuilder.Entity<Coupon>()
            .HasIndex(c => c.UserId);

        modelBuilder.Entity<Coupon>()
            .HasIndex(c => new { c.IsUsed, c.IsActive, c.ExpiresAt });

        // ============================================
>>>>>>> main
        // QUERY FILTERS - SOFT DELETE
        // ============================================

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        modelBuilder.Entity<Brand>()
            .HasQueryFilter(b => !b.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Order>()
            .HasQueryFilter(o => !o.IsDeleted);

        modelBuilder.Entity<Supplier>()
            .HasQueryFilter(s => !s.IsDeleted);

        // ============================================
        // ÍNDICES PARA MEJORAR RENDIMIENTO
        // ============================================
        modelBuilder.Entity<Product>()
            .HasIndex(p => p.IsDeleted);

        modelBuilder.Entity<Brand>()
            .HasIndex(b => b.IsDeleted); modelBuilder.Entity<Category>();

        modelBuilder.Entity<Category>()
            .HasIndex(c => c.IsDeleted);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.IsDeleted);

<<<<<<< HEAD
=======
        modelBuilder.Entity<StockMovement>()
            .HasIndex(sm => sm.CreatedAt);

        modelBuilder.Entity<StockMovement>()
            .HasIndex(sm => new { sm.ProductId, sm.CreatedAt });

        modelBuilder.Entity<StockAdjustment>()
            .HasIndex(sa => sa.AdjustmentDate);

        modelBuilder.Entity<SupplierClaim>()
            .HasIndex(sc => sc.Status);

>>>>>>> main
        // ============================================
        // ÍNDICES MODULO RECLAMOS
        // ============================================
        modelBuilder.Entity<Claim>()
            .HasIndex(r => r.NumeroTicket)
            .IsUnique();

        modelBuilder.Entity<Claim>()
            .HasIndex(r => r.ClienteId);

        modelBuilder.Entity<Claim>()
            .HasIndex(r => r.Estado);

        modelBuilder.Entity<Claim>()
            .HasIndex(r => r.NivelUrgencia);

        modelBuilder.Entity<Claim>()
            .HasIndex(r => r.FechaCreacion);

        modelBuilder.Entity<Claim>()
            .HasIndex(r => new { r.Estado, r.NivelUrgencia, r.FechaCreacion });

        modelBuilder.Entity<MessageClaim>()
            .HasIndex(m => m.ReclamoId);

        modelBuilder.Entity<MessageClaim>()
            .HasIndex(m => m.FechaEnvio);

        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.IsDeleted);
    }

    // ============================================
    // TABLAS EN BASE DE DATOS
    // ============================================
    public DbSet<AutoPartesRazor.Models.Claim> Reclamo { get; set; }
    public DbSet<AutoPartesRazor.Models.MessageClaim> MensajeReclamo { get; set; }
    public DbSet<AutoPartesRazor.Models.Product> Products { get; set; } = default!;
    public DbSet<AutoPartesRazor.Models.Brand> Brands { get; set; }
    public DbSet<AutoPartesRazor.Models.Category> Categories { get; set; }
    public DbSet<AutoPartesRazor.Models.Cart> Carts { get; set; }
    public DbSet<AutoPartesRazor.Models.User> Users { get; set; }
    public DbSet<AutoPartesRazor.Models.Order> Orders { get; set; }
    public DbSet<AutoPartesRazor.Models.OrderItem> OrderItems { get; set; }
    public DbSet<AutoPartesRazor.Models.Notification> Notifications { get; set; }
    public DbSet<AutoPartesRazor.Models.Supplier> Suppliers { get; set; }
    public DbSet<AutoPartesRazor.Models.ProductSupplier> ProductSuppliers { get; set; }
    public DbSet<AutoPartesRazor.Models.PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<AutoPartesRazor.Models.ProductReview> ProductReviews { get; set; } = default!;
    public DbSet<AutoPartesRazor.Models.ReviewHelpful> ReviewHelpfuls { get; set; } = default!;
<<<<<<< HEAD
=======
    public DbSet<AutoPartesRazor.Models.StockMovement> StockMovements { get; set; }
    public DbSet<AutoPartesRazor.Models.StockAdjustment> StockAdjustments { get; set; }
    public DbSet<AutoPartesRazor.Models.SupplierClaim> SupplierClaims { get; set; }
    public DbSet<AutoPartesRazor.Models.Coupon> Coupons { get; set; } = default!;
>>>>>>> main


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