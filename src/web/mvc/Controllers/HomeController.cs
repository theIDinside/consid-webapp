using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using webapp.mvc.DataAccessLayer;

namespace webapp.mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly LibraryContext db;

    public HomeController(ILogger<HomeController> logger, LibraryContext ctx)
    {
        _logger = logger;
        db = ctx;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public JsonResult GetCategories() {
        var categories = db.categoryItems.ToList();
        return Json(categories.Select(i => new { categoryId = i.ID, categoryName = i.CategoryName }));
    }
}
