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
            .OnDelete(DeleteBehavior.Restrict); 

        // Relación Product - Category
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
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

        // Relación Order - User
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Cart - User
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Cart>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

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

        modelBuilder.Entity<ProductSupplier>()
            .HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId);

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

    }

    public DbSet<AutoPartesRazor.Models.Product> Product { get; set; }
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
    public DbSet<AutoPartesRazor.Models.Claim> Reclamo { get; set; }
    public DbSet<AutoPartesRazor.Models.MessageClaim> MensajeReclamo { get; set; }
=======
        // Y su índice
        modelBuilder.Entity<Supplier>()
            .HasIndex(s => s.IsDeleted);
    }

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
>>>>>>> b508d6adfc2d2ae6ef774a3ec13a2962fe5795bc

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