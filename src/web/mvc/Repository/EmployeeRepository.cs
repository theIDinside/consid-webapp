using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;

namespace webapp.mvc.Repository;

public class EmployeeRepository : ConsidRepository<Employee> {

    public const int EmployeeFilterNoManagers = 1;
    public const int EmployeeFilterManagers = 2;
    public const int EmployeeFilterCEO = 3;
    public const int EmployeeFilterAll = 4;

    public EmployeeRepository(LibraryContext contextReference) : base(contextReference) {

    }

    public IQueryable<Employee> GetEmployees() {
        return ctx.employees.Where(e => !e.IsManager);
    }

    public IQueryable<Employee> GetManagers() {
        return ctx.employees.Where(e => e.IsManager && !e.IsCEO);
    }

    public IQueryable<Employee> GetCEO() {
        return ctx.employees.Where(e => e.IsManager && e.IsCEO);
    }

    public IQueryable<Employee> GetAllEmployees(int page, int pageSize) {
        return GetAllQueryable();
    }
}