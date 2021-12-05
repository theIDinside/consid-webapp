using webapp.mvc.Repository.Interfaces;
using webapp.mvc.DataAccessLayer;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;

namespace webapp.mvc.Repository;

public abstract class ConsidRepository<T> : IAsyncRepository<T> where T : class {
    protected readonly ApplicationDbContext ctx;
    public ConsidRepository(ApplicationDbContext context) {
        ctx = context;
    }

    public void Add(T item) {
        ctx.Set<T>().Add(item);
    }

    public async Task AddAsync(T item) {
        await ctx.Set<T>().AddAsync(item);
    }

    public void AddRange(IEnumerable<T> list) {
        ctx.Set<T>().AddRange(list);
    }

    public IQueryable<T> Find(Expression<Func<T, bool>> expression) {
        return ctx.Set<T>().Where(expression);
    }

    public bool Any(Expression<Func<T, bool>> expression) {
        return ctx.Set<T>().Any(expression);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression) {
        return await ctx.Set<T>().AnyAsync(expression);
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

    public void Update(T entity) {
        ctx.Entry<T>(entity).State = EntityState.Modified;
    }
}