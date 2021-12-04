using webapp.mvc.Repository.Interfaces;
using webapp.mvc.DataAccessLayer;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;

namespace webapp.mvc.Repository;

public abstract class ConsidRepository<T> : IAsyncRepository<T> where T : class {
    protected readonly LibraryContext ctx;
    public ConsidRepository(LibraryContext context) {
        ctx = context;
    }

    public void Add(T item) {
        ctx.Set<T>().Add(item);
    }

    public void AddRange(IEnumerable<T> list) {
        ctx.Set<T>().AddRange(list);
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> expression) {
        return ctx.Set<T>().Where(expression);
    }

    public IEnumerable<T> GetAll() {
        return ctx.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() {
        return await ctx.Set<T>().ToListAsync();
    }

    public IQueryable<T> GetAllQueryable() {
        return ctx.Set<T>();
    }

    public T? GetItemByID(int id) {
        return ctx.Set<T>().Find(id);
    }

    public async Task<T?> GetItemByIDAsync(int id) {
        return await ctx.Set<T>().FindAsync(id);
    }

    public void Remove(T entity) {
        ctx.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> list) {
        ctx.Set<T>().RemoveRange(list);
    }

    public void SaveChanges() {
        ctx.SaveChanges();
    }

    public async Task SaveChangesAsync() {
        await ctx.SaveChangesAsync();
    }

    public void Update(T entity) {
        ctx.Entry<T>(entity).State = EntityState.Modified;
    }
}