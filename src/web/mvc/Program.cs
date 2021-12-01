using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using webapp.mvc.DataAccessLayer;
using Microsoft.EntityFrameworkCore.SqlServer;
using webapp.mvc.Loggers;
using webapp.mvc.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddDbContext<webapp.mvc.DataAccessLayer.LibraryContext>(options => {
    // NB(for consid): we've set up this database connection in appsettings.json.
    // If you need to test against your own database, change the values there (port, hostname etc)

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSession(options => {
    // the requirements document says, the filtering should stay alive while using the web app
    // this invalidates the session cookie after 1 hour of idle time.
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddLogging(cfg => {
    cfg.AddDebug();
    cfg.AddConsole();
    cfg.AddFileLogger(cfg => {
        builder.Configuration.GetSection("Logging").GetSection("FileLogging").Bind(cfg);
    });
});


// Technically, we could just have "InputRankService" here, which takes a list of options
// for what the co-efficient should be, But I've just provided the "WalmartService" here as an example
// of how we could inject different salary calculation behaviors. WalmartService doesn't do anything different than InputRank
// it just increases the CEO salary co-efficient. If we really wanted different behavior, we would tell the service to tell the controller (EmployeeController)
// to render another view, with possibly different inputs, for instance. That's beyond this "lab" though.
var salaryService = builder.Configuration.GetValue<String>("SalaryService");
Console.WriteLine($"Salary Service: [{salaryService}]");
switch (salaryService) {
    case "InputRankService":
        builder.Services.AddSingleton<ISalaryService, InputRankSalaryService>();
        Console.WriteLine($"Salary service will be set to InputRankSalaryService");
        break;
    case "WalmartService":
        builder.Services.AddSingleton<ISalaryService, WalmarSalaryService>();
        Console.WriteLine($"Salary service will be set to WalmarSalaryService");
        break;
    default:
        builder.Services.AddSingleton<ISalaryService, InputRankSalaryService>();
        Console.WriteLine($"Salary service will be set to [ERROR: Not Found - Defaulting to InputRankSalaryService]");
        break;
}

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    // We seed the database, if none exists.
    SeedDB.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
