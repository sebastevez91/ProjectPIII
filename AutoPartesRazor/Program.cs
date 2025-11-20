using AutoPartesRazor.Constants;
using AutoPartesRazor.Data;
using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<AutoPartesRazorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AutoPartesRazorContext") ?? throw new InvalidOperationException("Connection string 'AutoPartesRazorContext' not found.")));

builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequiredLength = 6;
    x.Password.RequireUppercase = false;
    x.SignIn.RequireConfirmedEmail = false;
    x.SignIn.RequireConfirmedAccount = false;
    x.SignIn.RequireConfirmedPhoneNumber = false;
}).AddEntityFrameworkStores<AutoPartesRazorContext>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<SeedDataIdentity>();

// Configurar autorización basada en claims
builder.Services.AddAuthorization(options =>
{
    // Políticas para Usuarios
    options.AddPolicy("Users.View", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Users.View));
    options.AddPolicy("Users.Create", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Users.Create));
    options.AddPolicy("Users.Edit", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Users.Edit));
    options.AddPolicy("Users.Delete", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Users.Delete));

    // Políticas para Productos
    options.AddPolicy("Products.View", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Products.View));
    options.AddPolicy("Products.Create", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Create));
    options.AddPolicy("Products.Edit", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Edit));
    options.AddPolicy("Products.Delete", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Delete));

    // Políticas para Órdenes
    options.AddPolicy("Orders.View", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Orders.View));
    options.AddPolicy("Orders.Create", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Orders.Create));
    options.AddPolicy("Orders.Edit", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Orders.Edit));
    options.AddPolicy("Orders.Approve", policy =>
        policy.RequireClaim(Permissions.ClaimType, Permissions.Orders.Approve));

    // Agrega más políticas según necesites...
});

// Servicio de generación de PDFs
builder.Services.AddScoped<IPdfService, AutoPartesRazor.Services.PdfService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<IClaimService, ClaimService>();

var app = builder.Build();

// ============================================
// SEEDER - CARGAR DATOS DE PRUEBA
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        var context = services.GetRequiredService<AutoPartesRazorContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Aplicar migraciones pendientes
        logger.LogInformation("Verificando migraciones pendientes...");
        await context.Database.MigrateAsync();
        logger.LogInformation("✅ Migraciones aplicadas correctamente");

        // Limpiar carritos al iniciar
        logger.LogInformation("🧹 Limpiando carritos...");
        var carts = await context.Carts.ToListAsync();
        context.Carts.RemoveRange(carts);
        await context.SaveChangesAsync();
        logger.LogInformation($"✅ {carts.Count} carritos eliminados");

        // Solo ejecutar el seeder si la base está vacía
        if (!await context.Products.AnyAsync())
        {
            logger.LogInformation("🌱 Base de datos vacía. Iniciando carga de datos de prueba...");

            var seeder = new DatabaseSeeder(context, userManager);
            await seeder.SeedAsync();

            logger.LogInformation("✅ Datos de prueba cargados exitosamente");
        }
        else
        {
            logger.LogInformation("ℹ️ La base de datos ya contiene datos. Omitiendo seeder.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error al cargar datos de prueba: {Message}", ex.Message);
        // No lanzar la excepción para que la app siga ejecutándose
    }
}

SeedDataIdentity(app);
void SeedDataIdentity(WebApplication app)
{
    IServiceScopeFactory? scopedFactory =
    app.Services.GetService<IServiceScopeFactory>();
    using (IServiceScope scope = scopedFactory!.CreateScope())
    {
        SeedDataIdentity? service =
        scope.ServiceProvider.GetService<SeedDataIdentity>();
        service!.SeedAsync().Wait();
    }
}
// ============================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();