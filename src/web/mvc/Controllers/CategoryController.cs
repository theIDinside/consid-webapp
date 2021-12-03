using System.Data;
using System.Linq;
using System.Net;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webapp.mvc.Controllers;

public class CategoryController : Controller {
    private readonly ILogger<CategoryController> _logger;
    private LibraryContext db;

    public CategoryController(ILogger<CategoryController> logger, LibraryContext ctx) {
        _logger = logger;
        db = ctx;
    }

    // GET: Li
    public async Task<ActionResult> Index() {

        return View(await db.categoryItems.ToListAsync());
    }

    // GET: Returns the view containing the form for creating a Category
    public ActionResult Create() {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("CategoryName")] Category category) {
        if (await db.categoryItems.AnyAsync(cat => cat.CategoryName == category.CategoryName)) {
            ModelState.AddModelError("CategoryName", $"A category with name {category.CategoryName} already exists, you must choose another one.");
        }
        if (ModelState.IsValid) {
            db.categoryItems.Add(category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(category);
    }

    // GET: Get the view for editing a category
    public async Task<ActionResult> Edit(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        Category? category = await db.categoryItems.FindAsync(id);
        if (category == null) {
            return new NotFoundResult();
        }
        return View(category);
    }

    // POST: The actual operation for editing a category
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind("ID,CategoryName")] Category category) {
        if (await db.categoryItems.AnyAsync(c => c.CategoryName == category.CategoryName)) {
            ModelState.AddModelError("CategoryName", "A category with that name already exists");
        }
        if (ModelState.IsValid) {
            db.Entry(category).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(category);
    }

    // GET: Gets the view for deleting a category
    public async Task<ActionResult> Delete(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        Category? category = await db.categoryItems.FindAsync(id);
        if (category == null) {
            return new NotFoundResult();
        }

        return View(category);
    }

    // POST: The actual operation for deleteing a category, and the logic to prevent from deleting a referenced category
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id) {
        var category = await db.categoryItems.FindAsync(id);
        var hasEntitiesWithFKCategoryId = await db.libraryItems.AnyAsync(libitem => libitem.CategoryID == id);
        if (hasEntitiesWithFKCategoryId) {
            ModelState.AddModelError("CategoryName", "This category has items in it. You need to either delete those library items or move them to another category.");
        }
        if (ModelState.IsValid) {
            db.categoryItems.Remove(category);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(category);
    }

    // HTTP GET method for finding out how many items a specific category has
    [HttpGet]
    public async Task<JsonResult> GetCategoryItemCount(int id) {
        var itemCount = await db.libraryItems.Where(item => item.CategoryID == id).CountAsync();
        return Json(new { count = itemCount });
    }

    /**
     * A JSON GET METHOD. Returns a list of JSON objects { categoryId: number, categoryName: string } to the javascript / browser calling this route
     */
    [HttpGet]
    public async Task<JsonResult> GetCategories() {
        var categories = await db.categoryItems.ToListAsync();
        return Json(categories.Select(a => new { categoryId = a.ID, categoryName = a.CategoryName }));
    }


    protected override void Dispose(bool disposing) {
        if (disposing) {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
