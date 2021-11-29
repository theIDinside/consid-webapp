using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace webapp.mvc.Controllers;

public class CategoryController : Controller {
    private readonly ILogger<HomeController> _logger;
    private LibraryContext db;

    public CategoryController(ILogger<HomeController> logger, LibraryContext ctx)
    {
        _logger = logger;
        db = ctx;
    }

    [Route("/LibraryItem/Index")]
    public async Task<IActionResult> Index() {
        var items = await db.libraryItems.ToListAsync();
        _logger.LogInformation("Items:");
        if(items != null) {
            _logger.LogError("COULD NOT RETRIEVE DATA!");
        } else {
            foreach(var i in items) {
                _logger.LogInformation($"[{i.ID}] [{i.Title}] [{i.Author}] [{i.Pages ?? i.RunTimeMinutes}]");
            }
        }
        return View(items);
    }
}