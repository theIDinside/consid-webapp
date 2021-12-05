using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace webapp.mvc.Repository.Interfaces;

public interface IRepository<T> where T : class {
    T? GetItemByID(int id);
    IQueryable<T> GetAllQueryable();
    IQueryable<T> Find(Expression<Func<T, bool>> expression);
    void Add(T item);
    void Remove(T entity);
    void Update(T entity);
}

// For the repositories that would be async. In our application everything is async, so technically, this is overkill. I would not do this in production code unless
// there is a very good reason for it.
public interface IAsyncRepository<T> : IRepository<T> where T : class {
    Task<T?> GetItemByIDAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    Task AddAsync(T item);
}