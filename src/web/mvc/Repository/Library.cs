using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

public class Library : IDisposable {
    private readonly LibraryContext ctx;

    public LibraryItemRepository LibraryItems { get; private set; }
    public CategoryRepository Categories { get; private set; }

    public Library(LibraryContext contextReference) {
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

    public async Task<EditLibraryItemModel?> GetEditLibraryItemModel(int id) {
        var categories = await ctx.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        var it = await ctx.libraryItems.FirstAsync(i => i.ID == id);
        return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type, IsBorrowable = it.IsBorrowable, Borrower = it.Borrower, BorrowDate = it.BorrowDate };
    }

    public async Task<CreateLibraryItemModel> GetCreateLibraryItemModel() {
        var createLibItemViewModel = new CreateLibraryItemModel();
        createLibItemViewModel.Categories = await Categories.GetAllQueryable().Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        return createLibItemViewModel;
    }

}
