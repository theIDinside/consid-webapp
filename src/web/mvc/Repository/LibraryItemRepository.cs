using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

public class LibraryItemRepository : ConsidRepository<LibraryItem> {
    public LibraryItemRepository(LibraryContext contextReference) : base(contextReference) {

    }

    public async Task<PagedViewModel<LibraryItem>> GetPagedOrderBy(int page, int pageSize, string? filter, string? orderBy) {
        var items = from i in ctx.libraryItems select i;
        items = String.IsNullOrEmpty(filter) ? items.Include(e => e.Category) : items.Where(item => item.Title.Contains(filter)).Include(e => e.Category);
        items = (orderBy ?? "") switch {
            "cat" => items.OrderBy(i => i.Category.CategoryName),
            "cat_desc" => items.OrderByDescending(i => i.Category.CategoryName),
            "title" => items.OrderBy(i => i.Title),
            "title_desc" => items.OrderByDescending(i => i.Title),
            "type" => items.OrderBy(i => i.Type),
            "type_desc" => items.OrderByDescending(i => i.Type),
            _ => items
        };
        return await items.GetPagedAsync(page, pageSize);
    }
}