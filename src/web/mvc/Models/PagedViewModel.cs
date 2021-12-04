using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace webapp.mvc.Models;

// View model that wraps the Entity Framework Models returned by the controllers.
// Is used for pagination.
public abstract class BasePagedViewModel {
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int PageSize { get; }
    public int TotalRowCount { get; }

    public BasePagedViewModel(int pageIndex, int totalPages, int pageSize, int totalRowCount) {
        PageIndex = pageIndex;
        TotalPages = totalPages;
        PageSize = pageSize;
        TotalRowCount = totalRowCount;
    }

    // The rows are *not* 0-indexed, so [1 .. N], just like SQL views database tables. So first row is; row[1]
    public int FirstPageRow {
        get { return (PageIndex - 1) * PageSize + 1; }
    }

    public int LastPageRow {
        get { return Math.Min(PageIndex * PageSize, TotalRowCount); }
    }
}

public class PagedViewModel<T> : BasePagedViewModel where T : class {
    public List<T> Page { get; set; }
    public PagedViewModel(int index, int totalPageCount, int pageSize, int totalRowCount) : base(index, totalPageCount, pageSize, totalRowCount) {
        Page = new List<T>();
    }
}

// Extension function implementations. So that we can say someIQueryableItems.GetPagedAsync(somePageNum, itemsPerPage) and turn it into the kind of data we want to display on the view
public static class PagedQueryExtension {
    public async static Task<PagedViewModel<T>> GetPagedAsync<T>(this IQueryable<T> query, int page, int pageSize) where T : class {
        var totalRowCount = 0;
        // we try fast Count first. I'm not sure if this will do anything with SQL requests, but I would hope
        // the good people at Microsoft and those developing EF to know what they're doing for exposing this to the API
        // the TryGetNonEnumeratedCount first tries to get count, without "realizing" a list/result (say for instance, the type holds a field that has the value "Count" already, it will/should read that first)
        // thus making that count a lot faster
        if (!query.TryGetNonEnumeratedCount(out totalRowCount)) {
            totalRowCount = await query.CountAsync();
        }

        var totalPageCount = (int)Math.Ceiling((double)totalRowCount / pageSize);
        page = Math.Max(Math.Min(page, totalPageCount), 1);
        var result = new PagedViewModel<T>(page, totalPageCount, pageSize, totalRowCount);
        result.Page = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public static PagedViewModel<T> GetPaged<T>(this IQueryable<T> query, int page, int pageSize) where T : class {
        var totalRowCount = 0;
        // we try fast Count first. I'm not sure if this will do anything with SQL requests, but I would hope
        // the good people at Microsoft and those developing EF to know what they're doing for exposing this to the API
        // the TryGetNonEnumeratedCount first tries to get count, without "realizing" a list/result (say for instance, the type holds a field that has the value "Count" already, it will/should read that first)
        if (!query.TryGetNonEnumeratedCount(out totalRowCount)) {
            totalRowCount = query.Count();
        }

        var totalPageCount = (int)Math.Ceiling((double)totalRowCount / pageSize);
        var result = new PagedViewModel<T>(page, totalPageCount, pageSize, totalRowCount);
        result.Page = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return result;
    }
}