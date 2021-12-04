using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace webapp.mvc.Repository.Interfaces;

public interface IRepository<T> where T : class {
    T? GetItemByID(int id);
    IEnumerable<T> GetAll();
    IQueryable<T> GetAllQueryable();
    IQueryable<T> Find(Expression<Func<T, bool>> expression);
    void Add(T item);
    void AddRange(IEnumerable<T> list);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> list);
    void Update(T entity);

    void SaveChanges();
}

public interface IAsyncRepository<T> : IRepository<T> where T : class {
    Task<T?> GetItemByIDAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task SaveChangesAsync();
}