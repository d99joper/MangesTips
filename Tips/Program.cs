using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Tipset.Models;

var builder = WebApplication.CreateBuilder(args);

// Load secrets file (gitignored) — overrides placeholder values in appsettings.json
builder.Configuration.AddJsonFile("secrets/appsettings.Secrets.json", optional: true, reloadOnChange: false);

// Add MVC with views
builder.Services.AddControllersWithViews();

// Cookie authentication (migrated from Forms Authentication)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.Cookie.Name = "appNameAuth";
    });

builder.Services.AddAuthorization();

// Register EF Core DbContext with SQL Server and lazy loading proxies
var connStr = builder.Configuration.GetConnectionString("Tips_Entities")
    ?? throw new InvalidOperationException("Connection string 'Tips_Entities' not found.");

builder.Services.AddDbContext<Tips_Entities>(options =>
    options.UseSqlServer(connStr)
           .UseLazyLoadingProxies());
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<SettingsRepository>();
builder.Services.AddScoped<TeamRepository>();
builder.Services.AddScoped<MatchRepository>();
builder.Services.AddScoped<BlogRepository>();
builder.Services.AddScoped<TopScorerRepository>();

builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddSessionStateTempDataProvider();

builder.Services.AddDbContext<Tips_Entities>(options =>
    options.UseSqlServer(connStr, sqlOptions =>
        sqlOptions.CommandTimeout(300)) // 5 minutes
    .UseLazyLoadingProxies());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Health check stub
app.MapGet("/health", () => "ok");

app.Run();
