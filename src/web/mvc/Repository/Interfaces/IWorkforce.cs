using webapp.mvc.Models;
using webapp.mvc.Repository;

namespace mvc.Repository.Interfaces {
    // This interface, would basically make it possible to do testing, or just swap out "Library" backends. In production code
    // this interface would probably expose much more functionality
    public interface IWorkforce {
        IEmployeeRepository Employees { get; set; }
        // commits all changes made to the backend
        int Commit();
        // commits all changes made to the backend, async
        Task<int> CommitAsync();
        // dispose of the context
        void Dispose();
        bool CanPromoteToCEO(int id);
        List<Employee>? GetPossibleManagers(EmployeePosition position);
    }
}