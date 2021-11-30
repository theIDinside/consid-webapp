using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using webapp.mvc.DataAccessLayer;
using Microsoft.EntityFrameworkCore.SqlServer;
using webapp.mvc.Loggers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddDbContext<webapp.mvc.DataAccessLayer.LibraryContext>(options =>
{
    // NB(for consid): we've set up this database connection in appsettings.json.
    // If you need to test against your own database, change the values there (port, hostname etc)

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSession(options =>
{
    // the requirements document says, the filtering should stay alive while using the web app
    // this invalidates the session cookie after 1 hour of idle time.
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddLogging(cfg =>
{
    cfg.AddDebug();
    cfg.AddConsole();
    cfg.AddFileLogger(cfg =>
    {
        builder.Configuration.GetSection("Logging").GetSection("FileLogging").Bind(cfg);
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // We seed the database, if none exists.
    SeedDB.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
