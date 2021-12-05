using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using mvc.Repository.Interfaces;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

// Basically the "Unit of work" pattern, but not exactly (but almost)
public class Library : IDisposable, ILibrary {
    private readonly ApplicationDbContext ctx;

    public ILibraryItemRepository LibraryItems { get; set; }
    public ICategoryRepository Categories { get; set; }

    public Library(ApplicationDbContext contextReference) {
        ctx = contextReference;
        LibraryItems = new LibraryItemRepository(ctx);
        Categories = new CategoryRepository(ctx);
    }


    // commits all changes made to the backend
    public int Commit() {
        return ctx.SaveChanges();
    }

    // commits all changes made to the backend, async
    public async Task<int> CommitAsync() {
        return await ctx.SaveChangesAsync();
    }

    // dispose of the context
    public void Dispose() {
        ctx.Dispose();
    }

    // helper method, that creats a view model, sort of like a " Edit-view-model" of the Library Item model
    public async Task<EditLibraryItemModel?> GetEditLibraryItemModel(int id) {
        var categories = await ctx.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        var it = await ctx.libraryItems.FirstAsync(i => i.ID == id);
        return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type, IsBorrowable = it.IsBorrowable, Borrower = it.Borrower, BorrowDate = it.BorrowDate };
    }

    // helper method, that creats a view model, sort of like a "create view model" of the Library Item model
    public async Task<CreateLibraryItemModel> GetCreateLibraryItemModel() {
        var createLibItemViewModel = new CreateLibraryItemModel();
        createLibItemViewModel.Categories = await Categories.GetAllQueryable().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        return createLibItemViewModel;
    }

}
