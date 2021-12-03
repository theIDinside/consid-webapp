using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Services;
namespace webapp.mvc.Controllers {
    public class EmployeesController : Controller {

        private readonly LibraryContext db;
        private readonly ILogger<EmployeesController> _logger;
        public EmployeesController(ILogger<EmployeesController> logger, LibraryContext ctx) {
            db = ctx;
            _logger = logger;
        }

        // Validates the employee info, so that creation and editing of an employee follows the rules laid out in the requirements document
        private void validateManagerIDAttribute(Employee employee) {
            Task.Run(async () => {
                if (employee.ManagerID != null) {
                    if (employee.ManagerID == employee.ID) {
                        ModelState.AddModelError("ManagerID", "An employee can not manage oneself");
                        return;
                    }
                    if (employee.IsCEO) {
                        ModelState.AddModelError("ManagerID", "A CEO can not be managed by someone");
                        return;
                    }
                    if (await db.employees.FirstOrDefaultAsync(e => e.ID == employee.ManagerID) is Employee man) {
                        if (!employee.IsManager && man.IsCEO) {
                            ModelState.AddModelError("ManagerID", $"Employees can not be managed by a CEO");
                        }
                        if (!man.IsManager) {
                            ModelState.AddModelError("ManagerID", $"Employee {man.FullName} (ID: {man.ID}) is not a manager.");
                        }
                    } else {
                        ModelState.AddModelError("ManagerID", $"No manager found with id {employee.ManagerID}");
                    }
                }
            }).Wait();
        }

        // Makes sure that this employee is not a CEO while there already exists one.
        private void validateUniqueness(Employee employee) {
            if (employee.IsCEO && db.employees.Any(e => e.IsCEO && e.ID != employee.ID)) {
                ModelState.AddModelError("IsCEO", "There can only be one CEO of the library");
            }

        }

        public const int EmployeeFilterNoManagers = 1;
        public const int EmployeeFilterManagers = 2;
        public const int EmployeeFilterCEO = 3;
        public const int EmployeeFilterAll = 4;
        // GET: Employees
        public async Task<ActionResult> Index(int? page, int? employeeFilter, [FromServices] PageSizeService pageSizeService) {
            // if no employee filter is passed; display all
            var type = employeeFilter ?? EmployeeFilterAll;
            type = type > EmployeeFilterAll ? EmployeeFilterAll : type;
            ViewBag.GroupBy = type;

            // thank god for pattern matching. C# has finally "arrived."
            var viewModel = type switch {
                EmployeeFilterNoManagers => await db.employees.Where(e => !e.IsManager).GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                EmployeeFilterManagers => await db.employees.Where(e => e.IsManager && !e.IsCEO).GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                EmployeeFilterCEO => await db.employees.Where(e => e.IsCEO).GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                _ => await db.employees.GetPagedAsync(page ?? 1, pageSizeService.PageSize) // default case, or EmployeeFilterAll
            };
            ViewBag.CurrentPage = viewModel.PageIndex;
            return View(viewModel);
        }

        // GET: Employees/Create
        [HttpGet]
        public ActionResult Create() {
            // Note to consid: Since I am new to C#, ASP and it's frameworks, I realize that praxis is that one uses the Model-View-ViewModel design
            // but, at some point, I have to hand in this assignment. With the understanding of asp core that I have now, after a week and a half, I would have
            // instead went with that designs. Instead, I handle a lot of this stuff with Javascript calling into controller actions from the client side
            return View();
        }

        // POST: Employees/Create
        // Creates an employee & stores it in the database.
        // It takes an implementation of ISalaryService with the attribute [FromServices] attribute, which means, via Dependency Injection in core, gets injected into this controller action
        // We could for instance, also have used our own "direct" D.I. by passing an implementer of the interface to the constructor, but the controller action D.I. is even more loosely coupled than that.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(int SalaryInput, string EmployeeType, int ManagerID, [Bind("FirstName, LastName")] Employee employee, [FromServices] ISalaryService salaryService) {
            EmployeeType employeeType;
            employee.ManagerID = (ManagerID == -1) ? null : ManagerID;
            if (Enum.TryParse(EmployeeType, out employeeType)) {
                switch (employeeType) {
                    case Models.EmployeeType.Employee:
                        employee.IsManager = false;
                        employee.IsCEO = false;
                        break;
                    case Models.EmployeeType.Manager:
                        employee.IsManager = true;
                        employee.IsCEO = false;
                        break;
                    case Models.EmployeeType.CEO:
                        employee.IsManager = true;
                        employee.IsCEO = true;
                        break;
                }
            } else {
                ViewBag.ErrorMessage = $"Employee type {EmployeeType} not recognized";
                return View(employee);
            }
            employee.Salary = salaryService.CalculateSalary(employeeType, SalaryInput);
            validateUniqueness(employee);
            validateManagerIDAttribute(employee);
            if (ModelState.IsValid) {
                db.employees.Add(employee);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new BadRequestResult();
            }
            Employee? employee = await db.employees.FindAsync(id);
            if (employee == null) {
                return new NotFoundResult();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string EmployeeType, int ManagerID, [Bind("ID,FirstName,LastName,Salary, ManagerID")] Employee employee) {
            EmployeeType employeeType;
            // the HTTP POST protocol can't post "null" values (it doesn't know what that is. It would post empty fields. We post -1 to represent "null")
            employee.ManagerID = (ManagerID == -1) ? null : ManagerID;
            if (Enum.TryParse(EmployeeType, out employeeType)) {
                switch (employeeType) {
                    case Models.EmployeeType.Employee:
                        employee.IsManager = false;
                        employee.IsCEO = false;
                        break;
                    case Models.EmployeeType.Manager:
                        employee.IsManager = true;
                        employee.IsCEO = false;
                        break;
                    case Models.EmployeeType.CEO:
                        employee.IsManager = true;
                        employee.IsCEO = true;
                        break;
                }
            } else {
                ModelState.AddModelError("EmployeeType", $"Employee type {EmployeeType} not recognized");
            }
            validateUniqueness(employee);
            validateManagerIDAttribute(employee);
            if (ModelState.IsValid) {
                db.Entry(employee).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new BadRequestResult();
            }
            Employee? employee = await db.employees.FindAsync(id);
            if (employee == null) {
                return new NotFoundResult();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            if (await db.employees.FindAsync(id) is Employee employee) {
                if (employee.IsCEO || employee.IsManager) {
                    if (await db.employees.AnyAsync(e => e.ManagerID == employee.ID)) {
                        ModelState.AddModelError("ManagerID", "This person manages other employees and therefore can't be deleted.");
                    }
                }
                if (ModelState.IsValid) {
                    db.employees.Remove(employee);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                } else {
                    return View(employee);
                }
            } else {
                return new NotFoundResult();
            }
        }

        /**
         * A JSON GET METHOD. Returns a list of JSON objects of managers with their ID and Fullname. This is called by Javascript code
         * when we want to populate a list of Managers (for instance in the UI where we we create or edit employees)
         */
        [HttpGet]
        public async Task<JsonResult> GetManagers() {
            var managers = await db.employees.Where(emp => emp.IsManager).ToListAsync();
            return Json(managers.Select(man => {
                return new { id = man.ID, name = $"{man.FirstName} {man.LastName}" };
            }));
        }

        // Used for asking questions about an employee's manager for instance.
        // This request must not fail, so when an invalid ID is sent (or any other error occurs) this returns an empty JSON object
        [HttpGet]
        public async Task<JsonResult> GetEmployeeInfo(int id) {
            var e = await db.employees.FirstOrDefaultAsync(e => e.ID == id);
            if (e != null) {
                var employeeType = (e.IsManager, e.IsCEO) switch {
                    (true, true) => "CEO",
                    (true, false) => "Manager",
                    _ => "Employee"
                };
                return Json(new { id = e.ID, firstname = e.FirstName, lastname = e.LastName, employeetype = employeeType, manager = e.ManagerID ?? -1 });
            } else {
                return Json(new { });
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
