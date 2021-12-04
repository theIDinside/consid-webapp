using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

public class CategoryRepository : ConsidRepository<Category> {
    public CategoryRepository(LibraryContext contextReference) : base(contextReference) {

    }

    public async Task<PagedViewModel<Category>> GetPagedOrderBy(int page, int pageSize, string? filter) {
        var items = from i in ctx.categoryItems select i;
        items = String.IsNullOrWhiteSpace(filter) ? items : items.Where(item => item.CategoryName.Contains(filter));
        return await items.GetPagedAsync(page, pageSize);
    }

    public async Task<EditLibraryItemModel?> GetEditLibraryItemModel(int id) {
        var categories = await ctx.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
        var it = await ctx.libraryItems.FirstAsync(i => i.ID == id);
        return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type, IsBorrowable = it.IsBorrowable, Borrower = it.Borrower, BorrowDate = it.BorrowDate };
    }
}