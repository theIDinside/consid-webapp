using webapp.mvc.Models;
using webapp.mvc.Repository.Interfaces;

namespace mvc.Repository.Interfaces {
    public interface IEmployeeRepository : IAsyncRepository<Employee> {
        IQueryable<Employee> GetEmployees();
        IQueryable<Employee> GetManagers();

        IQueryable<Employee> GetCEO();

        IQueryable<Employee> GetAllEmployees(int page, int pageSize);
    }
}