using DemoMVC.Data;
using DemoMVC.DataAccess.Repository;
using DemoMVC.DataAccess.Repository.IRepository;
using DemoMVC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using DemoMVC.Utility;
using Stripe;
using DemoMVC.DataAccess.DBInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection1")));

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IDBInitializer,DBInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
//builder.Services.AddAuthentication().AddFacebook(options => { options.AppId = "299491202838203"; options.AppSecret = "e8de09ccefe83896900448eb75734963"; });
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
StripeConfiguration.ApiKey= builder.Configuration.GetSection("Stripe:SecretKey").Get<string>(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=MainPage}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbinitialize= scope.ServiceProvider.GetRequiredService<IDBInitializer>();
        dbinitialize.Initialize();
    }
}