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

    // Returns an IQueryable, since it's a non-realized fetch from the backend, and we realize it, on the callee side when we need it.
    // that way, we can utilize our extension method for IQueryable<T>; items.GetPaged(page, pageSize) to provide a "paged" user interface
    public IQueryable<LibraryItem> QueryOrderBy(string? filter, string? orderBy) {
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
        return items;
    }
}