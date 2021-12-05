using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using webapp.mvc.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace webapp.mvc.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext db;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext ctx) {
        _logger = logger;
        db = ctx;
    }

    public IActionResult Index() {
        return View();
    }

    public IActionResult Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
