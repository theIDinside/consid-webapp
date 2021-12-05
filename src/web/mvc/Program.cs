using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using webapp.mvc.DataAccessLayer;
using Microsoft.EntityFrameworkCore.SqlServer;
using webapp.mvc.Loggers;
using webapp.mvc.Services;
using webapp.mvc.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddDbContext<webapp.mvc.DataAccessLayer.LibraryContext>(options => {
    // NB(for consid): we've set up this database connection in appsettings.Development.json.
    // If you need to test against your own database, change the values there (port, hostname etc).
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// adds the session service. We set some options for how long a session stays alive (when idle. When not idle, it's persistent)
builder.Services.AddSession(options => {
    // the requirements document says, the filtering should stay alive while using the web app
    // this invalidates the session cookie after 1 hour of idle time.
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Playing around with some logging services to find out what SQL queries that the entity framework end up doing.
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
// to render another view, with possibly different inputs, for instance. That's beyond this "lab" though, also, I'm not familiar enough with C# and it's ecosystem,
// to pull that off nicely, but I've left this here, as an indicator of what my technical potential would be.
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

// So that we can set the page size in appsettings.Development.json, or appsettings.Release.json, or appsettings.json
// this is not technically how you're supposed to use services, but I made this quick hack, so that you can change the settings, without rebuilding the project, just
// change the value PageSize in appsettings.Development.json to whatever number you want and there you go.
var pageSize = builder.Configuration.GetValue<int>("PageSize");
pageSize = (pageSize < 0) ? 5 : pageSize;
var pageSizeService = new PageSizeService(pageSize);

// register page size serivce; so that we can set the amount of items per page, in the appsettings.json
builder.Services.AddSingleton<PageSizeService>(pageSizeService);
// Register our "unit of work"-like services that talks to the backend. It's not fully a unit of work pattern, but sorta
builder.Services.AddTransient<Library>();
builder.Services.AddTransient<Workforce>();

// build the app, with the configurations and settings we've provided, the services etc
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
// we want to use static files in wwwroot
app.UseStaticFiles();
// we want to use routing
app.UseRouting();
// we want to be able to store cookies, which is used for keeping track of the ordering of library items
// this makes it stay alive during the use of the application (until the user closes the window or goes idle)
app.UseSession();

app.UseAuthorization();
// define default route patterns, such as /LibraryItem/Index, LibraryItem/Edit/4, with similar patterns for employees
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
