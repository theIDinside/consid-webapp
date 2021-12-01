using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using webapp.mvc.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace webapp.mvc.Controllers;

public class HomeController : Controller {
    private readonly ILogger<HomeController> _logger;
    private readonly LibraryContext db;

    public HomeController(ILogger<HomeController> logger, LibraryContext ctx) {
        _logger = logger;
        db = ctx;
    }

    public async Task<IActionResult> Index() {
        return View();
    }

    public async Task<IActionResult> Privacy() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<JsonResult> GetCategories() {
        var categories = await db.categoryItems.ToListAsync();
        return Json(categories.Select(i => new { categoryId = i.ID, categoryName = i.CategoryName }));
    }

    [HttpGet]
    public async Task<JsonResult> GetItems() {
        var items = await db.libraryItems.ToListAsync();
        return Json(items.Select(i => new { title = i.Title, author = i.Author }));
    }

}
