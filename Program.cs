using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RangShala;
using RangShala.Data;
using RangShala.Models;
using RangShala.Services; // For EmailService
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add User Database Context (RangShalaDB)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Admin Database Context (RangShalaAdminDB)
builder.Services.AddDbContext<AdminDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AdminDbConnection")));

// Add EmailService
builder.Services.AddScoped<EmailService>(); // Register EmailService

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Razorpay configuration
builder.Services.Configure<RazorpaySettings>(builder.Configuration.GetSection("Razorpay"));
// Register RazorpayClient as a scoped service
builder.Services.AddScoped<Razorpay.Api.RazorpayClient>(sp =>
{
    var settings = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<RazorpaySettings>>().Value;
    if (string.IsNullOrEmpty(settings.KeyId) || string.IsNullOrEmpty(settings.KeySecret))
    {
        throw new InvalidOperationException("Razorpay KeyId or KeySecret is not configured in appsettings.json.");
    }
    return new Razorpay.Api.RazorpayClient(settings.KeyId, settings.KeySecret);
});

// Add configuration for design-time migrations (optional, but ensures consistency)
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(app.Environment.WebRootPath, "Images")),
    RequestPath = "/Images"
});

app.UseRouting();
app.UseSession();
app.UseAuthorization();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Application Started Successfully.");

// Apply Migrations for Both Databases & Seed Default Admin and Artworks
using (var scope = app.Services.CreateScope())
{
    try
    {
        var userDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var adminDbContext = scope.ServiceProvider.GetRequiredService<AdminDbContext>();

        if (userDbContext.Database.GetPendingMigrations().Any())
        {
            userDbContext.Database.Migrate();
            logger.LogInformation("User database migrations applied successfully.");
        }

        if (adminDbContext.Database.GetPendingMigrations().Any())
        {
            adminDbContext.Database.Migrate();
            logger.LogInformation("Admin database migrations applied successfully.");
        }

        if (!adminDbContext.Admins.Any())
        {
            adminDbContext.Admins.Add(new RangShala.Models.Admin
            {
                Email = "ayushibabariya4@gmail.com",
                Password = "Ayushi@123"
            });
            adminDbContext.SaveChanges();
            logger.LogInformation("Default admin created successfully.");
        }

        if (!adminDbContext.Artworks.Any())
        {
            var defaultArtworks = new[]
            {
                new Artwork { PaintingName = "Shree Nathji", PaintingImage = "/Images/Shreenathji.jpg", PaintingPrice = 4500, ArtistName = "Ayushi Babariya", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Gouache Painting", PaintingImage = "/Images/gouachepainting.jpg", PaintingPrice = 1000, ArtistName = "Ayushi Babariya", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Ganpati Bappa", PaintingImage = "/Images/Ganpatibappa.jpg", PaintingPrice = 700, ArtistName = "Niyati Agravat", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Night Pai", PaintingImage = "/Images/NightPai.jpg", PaintingPrice = 500, ArtistName = "Niyati Agravat", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "House Painting", PaintingImage = "/Images/HousePainting.jpg", PaintingPrice = 700, ArtistName = "Niyati Agravat", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Rainy Day", PaintingImage = "/Images/RainyDay.jpg", PaintingPrice = 1000, ArtistName = "Ayushi Babariya", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Hill View", PaintingImage = "/Images/HillView.jpg", PaintingPrice = 700, ArtistName = "Niyati Agravat", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Hanuman Dada", PaintingImage = "/Images/HanumanDada.jpg", PaintingPrice = 300, ArtistName = "Ayushi Babariya", Category = "Acrylic Painting" },
                new Artwork { PaintingName = "Sunrise Painting", PaintingImage = "/Images/SunrisePainting.jpg", PaintingPrice = 700, ArtistName = "Niyati Agravat", Category = "Acrylic Painting" }
            };
            adminDbContext.Artworks.AddRange(defaultArtworks);
            userDbContext.Artworks.AddRange(defaultArtworks);
            adminDbContext.SaveChanges();
            userDbContext.SaveChanges();
            logger.LogInformation("Default artworks seeded successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError($"Error applying database migrations or seeding data: {ex.Message}");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();