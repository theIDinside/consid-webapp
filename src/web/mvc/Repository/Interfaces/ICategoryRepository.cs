using webapp.mvc.Models;
using webapp.mvc.Repository.Interfaces;

namespace mvc.Repository.Interfaces {
    public interface ICategoryRepository : IAsyncRepository<Category> {
        public IQueryable<Category> GetAllFilterBy(string? filter);
    }
}