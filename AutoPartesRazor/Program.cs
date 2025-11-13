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
    x.Password.RequiredLength = 8;
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

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
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

app.MapControllers();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
