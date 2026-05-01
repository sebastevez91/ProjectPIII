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
// Servicio de Reclamo
builder.Services.AddScoped<IClaimService, ClaimService>();

var app = builder.Build();

// ============================================
// SEED DE DATOS (Solo en Development)
// ============================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AutoPartesRazorContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        var seeder = new DatabaseSeeder(context, userManager, roleManager);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "❌ Ocurrió un error al ejecutar el seed de datos");
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