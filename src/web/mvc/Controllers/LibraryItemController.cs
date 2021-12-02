using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webapp.mvc.Controllers;
/*
    Internal research notes 
    HTTP Request comes in and goes to
                |-> Middleware
                |-> Routing
                |-> Controller initialization (this is where the factory comes to play)
                |-> Action Method Execution
                |                     -----------> Data result sent out
                |                   /
                |-> Result Execution
                                    \
                                     \-> View result
                                     |-> View Rendering -> Response sent out

*/
public class LibraryItemController : Controller {
    private readonly ILogger<LibraryItemController> _logger;
    private LibraryContext db;

    private String SessionOrdering {
        get {
            if (this.HttpContext.Session.GetString("Ordering") == null) {
                this.SessionOrdering = "cat";
            }
            return this.HttpContext.Session.GetString("Ordering") ?? "cat";
        }
        set { this.HttpContext.Session.SetString("Ordering", value); }
    }

    public LibraryItemController(ILogger<LibraryItemController> logger, LibraryContext ctx) {

        _logger = logger;
        db = ctx;
    }

    [Route("/LibraryItem/Index")]
    public async Task<IActionResult> Index(string sortBy, string searchString, int? page) {
        SessionOrdering = String.IsNullOrEmpty(sortBy) ? SessionOrdering : sortBy;

        ViewBag.Ordering = SessionOrdering;
        var items = from i in db.libraryItems select i;

        if (searchString != null) {
            page = 1;
        } else {
            searchString = ViewBag.Filter;
        }
        ViewBag.Filter = searchString;

        _logger.LogCritical($"Session ordering: {SessionOrdering} : Sort by: {sortBy}");
        items = String.IsNullOrEmpty(searchString) ? items.Include(e => e.Category) : items.Where(item => item.Title.Contains(searchString)).Include(e => e.Category);
        switch (SessionOrdering) {
            case "cat":
                items = items.OrderBy(i => i.Category.CategoryName);
                break;
            case "cat_desc":
                items = items.OrderByDescending(i => i.Category.CategoryName);
                break;
            case "title":
                items = items.OrderBy(i => i.Title);
                break;
            case "title_desc":
                items = items.OrderByDescending(i => i.Title);
                break;
            case "type":
                items = items.OrderBy(i => i.Type);
                break;
            case "type_desc":
                items = items.OrderByDescending(i => i.Type);
                break;
        }
        var result = await items.GetPagedAsync(page ?? 1, 5);
        _logger.LogDebug("Items:");
        if (result == null) {
            _logger.LogError("COULD NOT RETRIEVE DATA!");
        } else {
            foreach (var i in result.Page) {
                _logger.LogDebug($"[{i.ID}] [{i.Title}] [{i.Author}] [{i.Pages ?? i.RunTimeMinutes}]");
            }
        }
        return View(result);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Checkout(int ID, string Borrower, DateTime? BorrowDate) {
        if (await db.libraryItems.FindAsync(ID) is LibraryItem libraryItem) {
            if (libraryItem.Type == "reference book") {
                @ViewBag.EditErrorMessage = "You can't borrow a reference book";
                ModelState.AddModelError("Type", "You can't borrow a reference book");
            }
            if (ModelState.IsValid) {
                if (!String.IsNullOrWhiteSpace(Borrower) && BorrowDate != null) {
                    libraryItem.Borrower = Borrower;
                    libraryItem.BorrowDate = BorrowDate;
                    db.Entry(libraryItem).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                } else {
                    @ViewBag.EditErrorMessage = "You did not set a borrower and/or date";
                    return View(libraryItem);
                }
            }
            return View(libraryItem);
        } else {
            return new NotFoundResult();
        }

    }

    public async Task<ActionResult> CheckIn(int? id) {
        if (await db.libraryItems.FindAsync(id) is LibraryItem libraryItem) {
            libraryItem.BorrowDate = null;
            libraryItem.Borrower = "";
            if (ModelState.IsValid) {
                db.Entry(libraryItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        } else {
            return new NotFoundResult();
        }
    }

    // GET method
    public ActionResult Create() {
        // We return an empty view, because, we let our custom UI library handle the populating of fields (like the Categories drop down list). 
        return View();
    }

    // POST METHOD
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("CategoryID,Title,Author,Pages,RunTimeMinutes,IsBorrowable,Borrower,BorrowDate,Type")] LibraryItem libraryItem) {
        if (!libraryItem.BorrowDate.HasValue && libraryItem.Borrower == null) libraryItem.Borrower = ""; // as per requirement doc. for some reason, Borrower should not be nullable
        if (libraryItem.Type == "reference book" && libraryItem.BorrowDate.HasValue) {
            ModelState.AddModelError("Type", "Reference book can not be borrowed. Only books, dvd's and audio books can be borrowerd");
            @ViewBag.ErrorMessage = ""; //  "Reference book can not be borrwed";
                                        // return View(libraryItem);
        }
        if (!String.IsNullOrWhiteSpace(libraryItem.Borrower) && !libraryItem.BorrowDate.HasValue) {
            @ViewBag.ErrorMessage = ""; // "Borrow date was not set";
            ModelState.AddModelError("BorrowDate", "Borrow date was not set");
            // return View(libraryItem);
        }
        if (libraryItem.BorrowDate.HasValue && String.IsNullOrWhiteSpace(libraryItem.Borrower)) {
            @ViewBag.ErrorMessage = ""; // "Borrower did not have a name.";
            ModelState.AddModelError("Borrower", "Borrower lacked a name input");
            // return View(libraryItem);
        }
        var cat = await db.categoryItems.FindAsync(libraryItem.CategoryID);
        if (cat == null) return View(libraryItem);
        libraryItem.Category = cat;
        if (ModelState.IsValid) {
            db.libraryItems.Add(libraryItem);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return View(libraryItem);
    }

    // GET METHOD
    public async Task<ActionResult> Edit(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        LibraryItem? libraryItem = await db.libraryItems.FindAsync(id);
        if (libraryItem == null) {
            return new NotFoundResult();
        }
        var categoriesList = await (from c in db.categoryItems select c).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        ViewBag.CategoriesDropdownList = categoriesList;
        return View(libraryItem);
    }

    // POST METHOD
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> EditConfirmed(int ID, int CategoryID, string Title, string Author, int? Pages, int? RunTimeMinutes, bool IsBorrowable, string Borrower, DateTime? BorrowDate, string Type) {
        // NOTE(simon): Bug in ModelState validation? I've set this to "AllowEmptyStrings", I've tried setting it to "minimum length = 1" as well to no avail. All that's left
        // is clearing it manually. Yay OOP.
        ModelState.ClearValidationState("Borrower");
        var libraryItem = new LibraryItem { ID = ID, CategoryID = CategoryID, Title = Title, Author = Author, Pages = Pages, RunTimeMinutes = RunTimeMinutes, IsBorrowable = IsBorrowable, Borrower = Borrower, BorrowDate = BorrowDate, Type = Type };
        var cat = await db.categoryItems.FindAsync(libraryItem.CategoryID);
        if (cat == null) {
            ModelState.AddModelError("CategoryID", $"No category with id {libraryItem.CategoryID} exists");
        } else {
            libraryItem.Category = cat;
        }
        if (libraryItem.Type == "reference book" && libraryItem.BorrowDate.HasValue) {
            var old = await db.libraryItems.FindAsync(libraryItem.ID);
            if (old == null) {
                return new NotFoundResult();
            }
            var categoriesList = await (from c in db.categoryItems select c).Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            ViewBag.CategoriesDropdownList = categoriesList;
            ModelState.AddModelError("Type", "This book must be checked in first before it can be changed to a reference book");
        }

        if (!libraryItem.BorrowDate.HasValue) {
            libraryItem.Borrower = "";
        }
        TryValidateModel(libraryItem);
        if (ModelState.IsValid) {
            db.Entry(libraryItem).State = EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return View(await db.libraryItems.FindAsync(ID));
    }

    // GET METHOD
    public async Task<ActionResult> Delete(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        LibraryItem? libraryItem = await db.libraryItems.FindAsync(id);
        if (libraryItem == null) {
            return new NotFoundResult();
        }
        return View(libraryItem);
    }

    // POST METHOD
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id) {
        LibraryItem? libraryItem = await db.libraryItems.FindAsync(id);
        if (libraryItem != null)
            db.libraryItems.Remove(libraryItem);
        return await db.SaveChangesAsync().ContinueWith(t => RedirectToAction("Index"));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}