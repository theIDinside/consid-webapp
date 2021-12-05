using mvc.Repository.Interfaces;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;

namespace webapp.mvc.Repository;

// Basically the "Unit of work" pattern, but not exactly (but almost)
public class Workforce : IDisposable, IWorkforce {
    private readonly ApplicationDbContext ctx;
    public IEmployeeRepository Employees { get; set; }

    public Workforce(ApplicationDbContext contextReference) {
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

    public List<Employee>? GetPossibleManagers(EmployeePosition position) {
        switch (position) {
            case EmployeePosition.Employee:
                return Employees.GetManagers().ToList();
            case EmployeePosition.Manager:
                return Employees.GetManagers().Concat(Employees.GetCEO()).ToList();
        }
        return null;
    }
}
