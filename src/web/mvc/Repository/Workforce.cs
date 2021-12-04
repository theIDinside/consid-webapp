using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

// Basically the "Unit of work" pattern, but not exactly (but almost)
public class Workforce : IDisposable {
    private readonly LibraryContext ctx;
    public EmployeeRepository Employees { get; private set; }

    public Workforce(LibraryContext contextReference) {
        ctx = contextReference;
        Employees = new EmployeeRepository(ctx);
    }


    // commits all changes made to the backend
    public int Commit() {
        return ctx.SaveChanges();
    }

    // commits all changes made to the backend, async
    public async Task<int> CommitAsync() {
        return await ctx.SaveChangesAsync();
    }

    // dispose of the context
    public void Dispose() {
        ctx.Dispose();
    }

    public bool CanPromoteToCEO(int id) {
        return Employees.Find(e => e.IsCEO && e.ID != id).Count() == 0;
    }
}
