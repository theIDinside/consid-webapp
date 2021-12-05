using webapp.mvc.Models;
using webapp.mvc.Repository.Interfaces;

namespace mvc.Repository.Interfaces {
    public interface ILibraryItemRepository : IAsyncRepository<LibraryItem> {
        public IQueryable<LibraryItem> QueryOrderBy(string? filter, string? orderBy);
    }
}