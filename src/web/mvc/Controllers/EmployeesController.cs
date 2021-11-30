using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;

namespace ConsidTechAssessment.Controllers
{

    enum EmployeeTypeParameter
    {
        Employee,
        Manager,
        CEO
    }

    public class EmployeesController : Controller
    {

        private readonly LibraryContext db;
        private readonly ILogger<EmployeesController> _logger;
        public EmployeesController(ILogger<EmployeesController> logger, LibraryContext ctx)
        {
            db = ctx;
            _logger = logger;
        }

        // Depdendency injection can be good. I love dependency injection. But in order to do *real* D.I., passing an interface along to the constructor
        // I now have to implement IControllerFactory. Which means I have to know what those 3 methods do, in order to be efficient and keep the complexity down.
        // At some point, one must stop and ask oneself; how much complexity am I adding, for the sake of sticking to GoF patterns and SOLID, DRY, so on and so forth?
        // Let's not forget either; these things come with a pretty hefty cost. Virtual tables ever increasing in size, virtual dispatch adding multiple layers of indirection (and CPU cache misses)
        // Design is everything, obviously, but it also has to be non-pessimized design, otherwise we be slow. And if we're slow, we're ramping up electricity costs for the companies we're supposed
        // to do digitalization for. Just a little rant from me.
        private async void validateManagerIDAttribute(Employee employee)
        {
            if (employee.ManagerID == -1)
            {
                employee.ManagerID = null;
                return;
            }
            if (employee.ManagerID != null)
            {
                if (employee.ManagerID == employee.ID)
                {
                    ModelState.AddModelError("ManagerID", "An employee can not manage oneself");
                    return;
                }
                if (employee.IsCEO)
                {
                    ModelState.AddModelError("ManagerID", "A CEO can not be managed by someone");
                    return;
                }
                var man = await db.employees.FirstOrDefaultAsync(e => e.ID == employee.ManagerID);
                if (man != null)
                {
                    if (!man.IsManager)
                    {
                        ModelState.AddModelError("ManagerID", $"Employee {man.FullName} (ID: {man.ID}) is not a manager.");
                    }
                }
                else
                {
                    ModelState.AddModelError("ManagerID", $"No manager found with id {employee.ManagerID}");
                }
            }

        }

        // Validates an Employee Model object, that is on route for insertion into -> database.
        // Privately mutates the ModelState.
        private void validateUniqueness(Employee employee)
        {
            if (db.employees.Any(e => e.ID == employee.ID))
            {
                ModelState.AddModelError("ID", "Employee with this ID already exists");
            }
            if (employee.IsCEO && db.employees.Any(e => e.IsCEO && e.ID != employee.ID))
            {
                ModelState.AddModelError("IsCEO", "There can only be one CEO of the library");
            }

        }

        // GET: Employees
        public async Task<ActionResult> Index()
        {
            return View(await db.employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            Employee? employee = await db.employees.FindAsync(id);
            if (employee == null)
            {
                return new NotFoundResult();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int SalaryInput, string EmployeeType, int ManagerID, [Bind("FirstName, LastName")] Employee employee)
        {
            employee.ManagerID = ManagerID;
            EmployeeTypeParameter employeeType;
            if (Enum.TryParse(EmployeeType, out employeeType))
            {
                switch (employeeType)
                {
                    case EmployeeTypeParameter.Employee:
                        employee.IsManager = false;
                        employee.IsCEO = false;
                        break;
                    case EmployeeTypeParameter.Manager:
                        employee.IsManager = true;
                        employee.IsCEO = false;
                        break;
                    case EmployeeTypeParameter.CEO:
                        employee.IsManager = true;
                        employee.IsCEO = true;
                        break;
                }
            }
            else
            {
                ViewBag.ErrorMessage = $"Employee type {EmployeeType} not recognized";
                return View(employee);
            }
            validateUniqueness(employee);
            validateManagerIDAttribute(employee);
            if (ModelState.IsValid)
            {
                db.employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            Employee? employee = await db.employees.FindAsync(id);
            if (employee == null)
            {
                return new NotFoundResult();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int SalaryInput, string EmployeeType, int ManagerID, [Bind("ID,FirstName,LastName,ManagerID")] Employee employee)
        {
            EmployeeTypeParameter employeeType;
            if (Enum.TryParse(EmployeeType, out employeeType))
            {
                switch (employeeType)
                {
                    case EmployeeTypeParameter.Employee:
                        employee.IsManager = false;
                        employee.IsCEO = false;
                        break;
                    case EmployeeTypeParameter.Manager:
                        employee.IsManager = true;
                        employee.IsCEO = false;
                        break;
                    case EmployeeTypeParameter.CEO:
                        employee.IsManager = true;
                        employee.IsCEO = true;
                        break;
                }
            }
            else
            {
                ViewBag.ErrorMessage = $"Employee type {EmployeeType} not recognized";
                return View(employee);
            }
            validateUniqueness(employee);
            validateManagerIDAttribute(employee);
            if (ModelState.IsValid)
            {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            Employee? employee = await db.employees.FindAsync(id);
            if (employee == null)
            {
                return new NotFoundResult();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Employee? employee = await db.employees.FindAsync(id);
            if (employee != null)
            {
                db.employees.Remove(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                return new NotFoundResult();
            }
        }

        /**
         * A JSON GET METHOD. Returns a list of JSON objects of managers with their ID and Fullname
         */
        [HttpGet]
        public async Task<JsonResult> GetManagers()
        {
            var managers = await db.employees.Where(emp => emp.IsManager).ToListAsync();
            return Json(managers.Select(man =>
            {
                Console.WriteLine($"Manager requested: {man.FirstName} {man.LastName}");
                return new { id = man.ID, name = $"{man.FirstName} {man.LastName}" };
            }));
        }

        [HttpGet]
        public async Task<JsonResult> GetManagerName(int id)
        {
            var manager = await db.employees.Where(emp => emp.ID == id).FirstAsync();
            if (manager == null)
            {
                return Json(new { id = -1, name = "Unmanaged" });
            }
            else
            {
                return Json(new { id = manager.ID, name = manager.FullName });
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
