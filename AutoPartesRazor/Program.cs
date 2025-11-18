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

// Servicio de generación de PDFs
builder.Services.AddScoped<IPdfService, AutoPartesRazor.Services.PdfService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
// Servicio de Reclamo
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