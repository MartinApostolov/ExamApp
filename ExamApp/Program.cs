using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExamApp.Areas.Identity.Data;
using System;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultString") ?? throw new InvalidOperationException("Connection string 'DefaultString' not found.");

builder.Services.AddDbContext<ExamApplicationDbContext>(options =>
{
    //the change occurs here.
    //builder.cofiguration and not just configuration
    options.UseMySQL(connectionString);
});

builder.Services.AddDefaultIdentity<ExamApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ExamApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
