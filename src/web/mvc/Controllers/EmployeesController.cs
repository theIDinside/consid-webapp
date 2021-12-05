using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using webapp.mvc.DataAccessLayer;
using webapp.mvc.Models;
using webapp.mvc.Services;
using webapp.mvc.Repository;
using webapp.mvc.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using mvc.Repository.Interfaces;

namespace webapp.mvc.Controllers {
    public class EmployeesController : Controller {
        // this interface, makes for instance, mocking and testing possible, since we would just inject a mock'ed implementation at test time
        private readonly IWorkforce db;
        private readonly ILogger<EmployeesController> _logger;
        public EmployeesController(ILogger<EmployeesController> logger, Workforce ctx) {
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

                    if (await db.Employees.GetItemByIDAsync(employee.ManagerID.Value) is Employee man) {
                        if (!employee.IsManager && man.IsCEO) {
                            ModelState.AddModelError("ManagerID", $"Employees can not be managed by a CEO");
                        }
                        if (!man.IsManager) {
                            ModelState.AddModelError("ManagerID", $"Employee {man.FullName} (ID: {man.ID}) is not a manager.");
                        }
                    } else {
                        ModelState.AddModelError("ManagerID", $"No manager found with id {employee.ManagerID}");
                    }
                } else {
                    if (!employee.IsManager) {
                        ModelState.AddModelError("ManagerID", "This employee must be managed by a manager!");
                    }
                }
            }).Wait();
        }

        // Makes sure that this employee is not a CEO while there already exists one.
        private void validateUniqueness(Employee employee) {
            if (employee.IsCEO && !db.CanPromoteToCEO(employee.ID)) {
                ModelState.AddModelError("IsCEO", "There can only be one CEO of the library");
            }
        }

        private const int EmployeeFilterNoManagers = 1;
        private const int EmployeeFilterCEO = 3;
        private const int EmployeeFilterManagers = 2;
        private const int EmployeeFilterAll = 4;
        // GET: Employees
        public async Task<ActionResult> Index(int? page, int? employeeFilter, [FromServices] PageSizeService pageSizeService) {
            // if no employee filter is passed; display all
            var type = employeeFilter ?? EmployeeFilterAll;
            type = type > EmployeeFilterAll ? EmployeeFilterAll : type;
            ViewBag.GroupBy = type;

            var viewModel = type switch {
                EmployeeFilterNoManagers => await db.Employees.GetEmployees().GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                EmployeeFilterManagers => await db.Employees.GetManagers().GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                EmployeeFilterCEO => await db.Employees.GetCEO().GetPagedAsync(page ?? 1, pageSizeService.PageSize),
                _ => await db.Employees.GetAllQueryable().GetPagedAsync(page ?? 1, pageSizeService.PageSize) // default case, or EmployeeFilterAll
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
            Decimal Salary = 0;
            if (!salaryService.TryCalculateSalary(employeeType, SalaryInput, out Salary)) {
                ModelState.AddModelError("SalaryInput", $"Input rank must be between 1 and 10. Input: [{SalaryInput}]");
            }
            employee.Salary = Salary;
            validateUniqueness(employee);
            validateManagerIDAttribute(employee);
            if (ModelState.IsValid) {
                await db.Employees.AddAsync(employee);
                await db.CommitAsync();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new BadRequestResult();
            }
            Employee? employee = await db.Employees.GetItemByIDAsync(id ?? 0);
            if (employee == null) {
                return new NotFoundResult();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string EmployeeType, [Bind("ID,FirstName,LastName,ManagerID")] Employee employee) {
            ModelState.Remove("ManagerList");
            EmployeeType employeeType;
            // the HTTP POST protocol can't post "null" values (it doesn't know what that is. It would post empty fields. We post -1 to represent "null")
            employee.ManagerID = (employee.ManagerID == -1) ? null : employee.ManagerID;
            if (await db.Employees.GetItemByIDAsync(employee.ID) is Employee employeeRecord) {
                if (Enum.TryParse(EmployeeType, out employeeType)) {
                    // we can ignore the "non exhaustive pattern" warning here, because, we're actually using TryParse - it will go to the else branch if not OK
#pragma warning disable CS8524
                    (employee.IsManager, employee.IsCEO) = employeeType switch {
                        Models.EmployeeType.Employee => (false, false),
                        Models.EmployeeType.Manager => (true, false),
                        Models.EmployeeType.CEO => (true, true),
                    };
#pragma warning restore CS8524
                } else {
                    ModelState.AddModelError("EmployeeType", $"Employee type {EmployeeType} not recognized");
                    return View(employee);
                }
                // update the entity tracked by the Entity Framework
                employeeRecord.FirstName = employee.FirstName;
                employeeRecord.LastName = employee.LastName;
                employeeRecord.ManagerID = employee.ManagerID;
                employeeRecord.IsManager = employee.IsManager;
                employeeRecord.IsCEO = employee.IsCEO;

                validateUniqueness(employeeRecord);
                validateManagerIDAttribute(employeeRecord);
                if (!employeeRecord.IsManager) {
                    if (await db.Employees.AnyAsync(e => (e.ManagerID ?? 0) == employeeRecord.ID)) {
                        ModelState.AddModelError("EmployeeType", "This employee manages other employees, it can not be demoted");
                    }
                }
                if (ModelState.IsValid) {
                    db.Employees.Update(employeeRecord);
                    await db.CommitAsync();
                    return RedirectToAction("Index");
                }
                return View(employee);
            } else {
                return new NotFoundResult();
            }
        }

        // GET: Employees/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new BadRequestResult();
            }
            Employee? employee = await db.Employees.GetItemByIDAsync(id ?? 0);
            if (employee == null) {
                return new NotFoundResult();
            }
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            if (await db.Employees.GetItemByIDAsync(id) is Employee employee) {
                if (employee.IsCEO || employee.IsManager) {
                    if (await db.Employees.AnyAsync(e => e.ManagerID == employee.ID)) {
                        ModelState.AddModelError("ManagerID", "This person manages other employees and therefore can't be deleted.");
                    }
                }
                if (ModelState.IsValid) {
                    db.Employees.Remove(employee);
                    await db.CommitAsync();
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
            var managers = await db.Employees.GetManagers().ToListAsync();
            var allmanagers = managers.Concat(await db.Employees.GetCEO().ToListAsync());
            return Json(allmanagers.Select(man => {
                return new { id = man.ID, name = $"{man.FirstName} {man.LastName}" };
            }));
        }

        // Used for asking questions about an employee's manager for instance.
        // This request must not fail, so when an invalid ID is sent (or any other error occurs) this returns an empty JSON object
        [HttpGet]
        public async Task<JsonResult> GetEmployeeInfo(int id) {
            var e = await db.Employees.GetItemByIDAsync(id);
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
