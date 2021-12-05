using webapp.mvc.DataAccessLayer;

using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using webapp.mvc.Services;
using webapp.mvc.Repository;
using mvc.Repository.Interfaces;

namespace webapp.mvc.Controllers;
public class LibraryItemController : Controller {
    private readonly ILogger<LibraryItemController> _logger;

    // this interface, makes for instance, mocking and testing possible, since we would just inject a mock'ed implementation at test time
    private readonly ILibrary db;

    // The ordering of library items. Stays alive for the session or until a user idles out (after 1 hr)
    private String SessionOrdering {
        get {
            if (this.HttpContext.Session.GetString("Ordering") == null) {
                this.SessionOrdering = "cat";
            }
            return this.HttpContext.Session.GetString("Ordering")!;
        }
        set { this.HttpContext.Session.SetString("Ordering", value); }
    }

    public LibraryItemController(ILogger<LibraryItemController> logger, Library ctx) {
        _logger = logger;
        db = ctx;
    }

    [Route("/LibraryItem/Index")]
    public async Task<IActionResult> Index(string sortBy, string searchString, int? page, [FromServices] PageSizeService pageSizeService) {
        SessionOrdering = String.IsNullOrEmpty(sortBy) ? SessionOrdering : sortBy;
        ViewBag.CurrentPage = page ?? 1;
        ViewBag.Ordering = SessionOrdering;

        if (searchString != null) {
            ViewBag.CurrentPage = 1;
            ViewBag.Filter = searchString;
        } else {
            searchString = ViewBag.Filter;
        }

        // var viewModel = await db.LibraryItems.GetPagedOrderBy(page ?? 1, pageSizeService.PageSize, searchString, SessionOrdering);
        var viewModel = await db.LibraryItems.QueryOrderBy(searchString, SessionOrdering).GetPagedAsync(page ?? 1, pageSizeService.PageSize);
        ViewBag.CurrentPage = viewModel.PageIndex;
        _logger.LogDebug("Items:");
        if (viewModel == null) {
            _logger.LogError("COULD NOT RETRIEVE DATA!");
        } else {
            foreach (var i in viewModel.Page) {
                _logger.LogDebug($"[{i.ID}] [{i.Title}] [{i.Author}] [{i.Pages ?? i.RunTimeMinutes}]");
            }
        }
        return View(viewModel);
    }


    [HttpPost, ActionName("CheckOut")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CheckOut(int ID, string Borrower, DateTime? BorrowDate) {
        ModelState.Clear();
        if (await db.LibraryItems.GetItemByIDAsync(ID) is LibraryItem libraryItem) {
            if (libraryItem.Type == "reference book") {
                ModelState.AddModelError("Type", "You can't borrow a reference book");
            }
            if (String.IsNullOrWhiteSpace(Borrower)) {
                ModelState.AddModelError("Borrower", "You need to input name of borrower");
            }
            if (!BorrowDate.HasValue) {
                ModelState.AddModelError("BorrowDate", "You need to input name of borrower");
            }

            if (ModelState.IsValid) {
                libraryItem.Borrower = Borrower;
                libraryItem.BorrowDate = BorrowDate;
                db.LibraryItems.Update(libraryItem);
                await db.CommitAsync();
                return RedirectToAction("Index");
            }
            var editModel = await db.GetEditLibraryItemModel(ID);
            editModel!.ViewTab = "CheckOutTab";
            return View("Edit", editModel);
        } else {
            return new NotFoundResult();
        }
    }

    // Controller action that "returns" a library item
    [HttpGet, ActionName("CheckIn")]
    public async Task<ActionResult> CheckIn(int? id) {
        if (await db.LibraryItems.GetItemByIDAsync(id ?? -1) is LibraryItem libraryItem) {
            libraryItem.BorrowDate = null;
            libraryItem.Borrower = "";
            if (ModelState.IsValid) {
                db.LibraryItems.Update(libraryItem);
                await db.CommitAsync();
            }
            return RedirectToAction("Index");
        } else {
            return new NotFoundResult();
        }
    }

    // GET method
    [HttpGet]
    public async Task<ActionResult> Create() {
        // We return an empty view, because, we let our custom UI library handle the populating of fields (like the Categories drop down list).
        // Note to consid: Since I am new to C#, ASP and it's frameworks, I realize that praxis is that one uses the Model-View-ViewModel design
        // but, at some point, I have to hand in this assignment. With the understanding of asp core that I have now, after a week and a half, I would have
        // instead went with that designs. Instead, I handle a lot of this stuff with Javascript calling into controller actions from the client side
        var createLibItemViewModel = await db.GetCreateLibraryItemModel();
        var count = (createLibItemViewModel.Categories ?? new List<SelectListItem>()).Count();
        if (count == 0) {
            ModelState.AddModelError("Category", "There exists no categories. You must first create a category where you can add this item to.");
        }
        return View(createLibItemViewModel);
    }

    // POST METHOD
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind("Title, Author, Length, Type, CategoryID")] Models.ViewModels.CreateLibraryItemModel item) {
        ModelState.Remove("Categories");
        var libItem = item.ToLibraryItem();
        if (libItem == null) {
            ModelState.AddModelError("Type", "Unknown type was set");
        }

        if (ModelState.IsValid && libItem != null) {
            await db.LibraryItems.AddAsync(libItem);
            await db.CommitAsync();
            return RedirectToAction("Index");
        } else {
            item.Categories = await db.Categories.GetAllQueryable().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            if (item.Categories.Count() == 0) {
                ModelState.AddModelError("Category", "There exists no categories. You must first create a category where you can add this item to.");
            }
        }
        return View(item);
    }

    public async Task<ActionResult> Edit(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        var editModel = await db.GetEditLibraryItemModel(id.Value);
        if (editModel == null) return new NotFoundResult();
        editModel.ViewTab = "EditDetailsTab";
        return View(editModel);
    }

    // POST METHOD
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditConfirmed([Bind("ID, Title, Author, Length, Type, CategoryID")] EditLibraryItemModel model) {
        ModelState.Remove("Categories");
        var dbItem = await db.LibraryItems.GetItemByIDAsync(model.ID);
        if (dbItem == null) {
            ModelState.AddModelError("ID", $"No library item with that ID exists: {model.ID}");
            return View(model);
        }
        var cat = await db.Categories.GetItemByIDAsync(model.CategoryID);
        if (cat == null) {
            ModelState.AddModelError("CategoryID", $"No category with id {model.CategoryID} exists");
        }

        if (model.Type == "reference book" && dbItem.BorrowDate.HasValue) {
            ModelState.AddModelError("Type", "This book must be checked in first before it can be changed to a reference book");
        }

        TryValidateModel(model);
        ModelState.Remove("Categories");
        var wasBorrowable = dbItem.IsBorrowable;
        switch (model.Type) {
            case "reference book":
                dbItem.Pages = model.Length;
                dbItem.RunTimeMinutes = null;
                dbItem.IsBorrowable = false;
                break;
            case "book":
                dbItem.Pages = model.Length;
                dbItem.RunTimeMinutes = null;
                dbItem.IsBorrowable = true;
                break;
            case "audio book":
            case "dvd":
                dbItem.Pages = null;
                dbItem.RunTimeMinutes = model.Length;
                dbItem.IsBorrowable = true;
                break;
            default:
                ModelState.AddModelError("Type", $"Type {model.Type} does not exist");
                break;
        }
        if (ModelState.IsValid) {
            dbItem.Title = model.Title;
            dbItem.Author = model.Author;
            dbItem.Type = model.Type;
            dbItem.CategoryID = model.CategoryID;
            db.LibraryItems.Update(dbItem);
            await db.CommitAsync();
            return RedirectToAction("Index");
        } else {
            var categoriesList = await db.Categories.GetAllQueryable().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            model.IsBorrowable = wasBorrowable;
            model.Categories = categoriesList;
            model.ViewTab = "EditDetailsTab";
            _logger.LogError("MODEL TYPE ERROR [{0}] db Type: {1}", model.Type, dbItem.Type);
            return View(model);
        }
    }

    // POST METHOD
    [HttpPost, ActionName("FooEdit")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> FooEditConfirmed(int ID, int CategoryID, string Title, string Author, int? Pages, int? RunTimeMinutes, bool IsBorrowable, string Borrower, DateTime? BorrowDate, string Type) {
        // NOTE(simon): Bug in ModelState validation? I've set this to "AllowEmptyStrings", I've tried setting it to "minimum length = 1" as well to no avail. All that's left
        // is clearing it manually. Yay OOP.
        ModelState.ClearValidationState("Borrower");
        var libraryItem = new LibraryItem { ID = ID, CategoryID = CategoryID, Title = Title, Author = Author, Pages = Pages, RunTimeMinutes = RunTimeMinutes, IsBorrowable = IsBorrowable, Borrower = Borrower, BorrowDate = BorrowDate, Type = Type };
        var cat = await db.Categories.GetItemByIDAsync(libraryItem.CategoryID);
        if (cat == null) {
            ModelState.AddModelError("CategoryID", $"No category with id {libraryItem.CategoryID} exists");
        } else {
            libraryItem.Category = cat;
        }
        if (libraryItem.Type == "reference book" && libraryItem.BorrowDate.HasValue) {
            var old = await db.LibraryItems.GetItemByIDAsync(libraryItem.ID);
            if (old == null) {
                return new NotFoundResult();
            }
            var categoriesList = await db.Categories.GetAllQueryable().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync(); ViewBag.CategoriesDropdownList = categoriesList;
            ModelState.AddModelError("Type", "This book must be checked in first before it can be changed to a reference book");
        }

        if (!libraryItem.BorrowDate.HasValue) {
            libraryItem.Borrower = "";
        }
        TryValidateModel(libraryItem);
        if (ModelState.IsValid) {
            db.LibraryItems.Update(libraryItem);
            await db.CommitAsync();
            return RedirectToAction("Index");
        }
        return View(await db.LibraryItems.GetItemByIDAsync(ID));
    }

    // GET METHOD
    public async Task<ActionResult> Delete(int? id) {
        if (id == null) {
            return new BadRequestResult();
        }
        LibraryItem? libraryItem = await db.LibraryItems.GetItemByIDAsync(id ?? 0);
        if (libraryItem == null) {
            return new NotFoundResult();
        }
        return View(libraryItem);
    }

    // POST METHOD
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(int id) {
        LibraryItem? libraryItem = await db.LibraryItems.GetItemByIDAsync(id);
        if (libraryItem != null)
            db.LibraryItems.Remove(libraryItem);
        return await db.CommitAsync().ContinueWith(t => RedirectToAction("Index"));
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            db.Dispose();
        }
        base.Dispose(disposing);
    }
}