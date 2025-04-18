using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebApplication_AuthenticationSystem_.Areas.Identity.Data;
using WebApplication_AuthenticationSystem_.Data;

var builder = WebApplication.CreateBuilder(args);

// Получение строки подключения из appsettings.json
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

// Регистрация DbContext с использованием строки подключения
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

// Настройка Identity с использованием Entity Framework
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<AuthDbContext>();

// Добавление остальных сервисов
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

var app = builder.Build();

// Настройка конвейера запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
