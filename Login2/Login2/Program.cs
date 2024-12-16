using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Login2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DuyuruService to the DI container
builder.Services.AddTransient<DuyuruService>();

// Session hizmetini ekleyin
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Diðer middleware'lerden önce session middleware'ini ekleyin
app.UseSession();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Özel bir endpoint ekleyin
    endpoints.MapGet("/Jsondb/{fileName}", async context =>
    {
        var fileName = (string)context.Request.RouteValues["fileName"];
        var filePath = Path.Combine("Jsondb", fileName); // Dosyanýn bulunduðu dizin

        if (File.Exists(filePath))
        {
            context.Response.ContentType = "application/json";
            await context.Response.SendFileAsync(filePath);
        }
        else
        {
            context.Response.StatusCode = 404;
        }
    });
});



app.Run();
