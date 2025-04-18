using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using WebApplication_AuthenticationSystem_.Areas.Identity.Data;
using WebApplication_AuthenticationSystem_.Data;

var builder = WebApplication.CreateBuilder(args);

// ��������� ������ ����������� �� appsettings.json
var connectionString = builder.Configuration.GetConnectionString("AuthDbContextConnection")
    ?? throw new InvalidOperationException("Connection string 'AuthDbContextConnection' not found.");

// ����������� DbContext � �������������� ������ �����������
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionString));

// ��������� Identity � �������������� Entity Framework
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>options.SignIn.RequireConfirmedAccount = true)
.AddEntityFrameworkStores<AuthDbContext>();

// ���������� ��������� ��������
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
});

var app = builder.Build();

// ��������� ��������� ��������
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
