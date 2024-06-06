using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;
using TASKManagementSystem_DAL.Data;
using TASKManagementSystem_DAL.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 7;
    options.Password.RequireUppercase = true;

    options.User.RequireUniqueEmail = true;
})
             .AddEntityFrameworkStores<AppDbContext>()
             .AddTokenProvider<DataProtectorTokenProvider<AppUser>>(TokenOptions.DefaultProvider);

// File provider for static files
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
              Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

// Add session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);//You can set Time   
});

// Add MVC
builder.Services.AddControllersWithViews();

// Add Identity
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireEmail", policy => policy.RequireClaim(ClaimTypes.Email));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseSession();

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();
