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
using webapp.mvc.Services;
using webapp.mvc.Repository;
using mvc.Repository.Interfaces;

namespace webapp.mvc.Controllers;

public class CategoryController : Controller {
    private readonly ILogger<CategoryController> _logger;
    // this interface, makes for instance, mocking and testing possible, since we would just inject a mock'ed implementation at test time
    private readonly ILibrary db;

    public CategoryController(ILogger<CategoryController> logger, Library ctx) {
        _logger = logger;
        db = ctx;
    }

    // GET: Li
    public async Task<ActionResult> Index(string searchString, int? page, [FromServices] PageSizeService pageSizeService) {
        ViewBag.CurrentPage = page ?? 1;
        if (searchString != null) {
            ViewBag.CurrentPage = 1;
            ViewBag.Filter = searchString;
        } else {
            searchString = ViewBag.Filter;
        }
        return View(await db.Categories.GetAllFilterBy(searchString).GetPagedAsync(page ?? 1, pageSizeService.PageSize));
    }

    // GET: Returns the view containing the form for creating a Category
    public ActionResult Create() {
        return View();
    }

    // POST: Category/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("CategoryName")] Category category) {
        if (await db.Categories.AnyAsync(cat => cat.CategoryName == category.CategoryName)) {
            ModelState.AddModelError("CategoryName", $"A category with name {category.CategoryName} already exists, you must choose another one.");
        }
        if (ModelState.IsValid) {
            await db.Categories.AddAsync(category);
            await db.CommitAsync();
            return RedirectToAction("Index");
        }

        return View(category);
    }

    // GET: Get the view for editing a category
    public async Task<ActionResult> Edit(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        Category? category = await db.Categories.GetItemByIDAsync(id ?? 0);
        if (category == null) {
            return new NotFoundResult();
        }
        return View(category);
    }

    // POST: The actual operation for editing a category
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind("ID,CategoryName")] Category category) {
        if (await db.Categories.AnyAsync(c => c.CategoryName == category.CategoryName)) {
            ModelState.AddModelError("CategoryName", "A category with that name already exists");
        }
        if (ModelState.IsValid) {
            db.Categories.Update(category);
            await db.CommitAsync();
            return RedirectToAction("Index");
        }
        return View(category);
    }

    // GET: Gets the view for deleting a category
    public async Task<ActionResult> Delete(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        Category? category = await db.Categories.GetItemByIDAsync(id ?? 0);
        if (category == null) {
            return new NotFoundResult();
        }

        return View(category);
    }

    // POST: The actual operation for deleteing a category, and the logic to prevent from deleting a referenced category
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id) {
        var category = await db.Categories.GetItemByIDAsync(id);
        var hasEntitiesWithFKCategoryId = await db.LibraryItems.AnyAsync(libitem => libitem.CategoryID == id);
        if (hasEntitiesWithFKCategoryId) {
            ModelState.AddModelError("ID", "This category has items in it. You need to either delete those library items or move them to another category.");
        }
        if (category == null) {
            ModelState.AddModelError("ID", "This category doesn't exist anymore");
        }
        if (ModelState.IsValid) {
            db.Categories.Remove(category!);
            await db.CommitAsync();
            return RedirectToAction("Index");
        }
        return View(category);
    }

    // HTTP GET method for finding out how many items a specific category has
    [HttpGet]
    public async Task<JsonResult> GetCategoryItemCount(int id) {
        var itemCount = await db.LibraryItems.Find(item => item.CategoryID == id).CountAsync();
        return Json(new { count = itemCount });
    }

    /**
     * A JSON GET METHOD. Returns a list of JSON objects { categoryId: number, categoryName: string } to the javascript / browser calling this route
     */
    [HttpGet]
    public async Task<JsonResult> GetCategories() {
        var categories = await db.Categories.GetAllAsync();
        return Json(categories.Select(a => new { categoryId = a.ID, categoryName = a.CategoryName }));
    }


    protected override void Dispose(bool disposing) {
        if (disposing) {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}
