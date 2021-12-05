using webapp.mvc.Models;
namespace webapp.mvc.Services;

// The implementation of the interface ISalaryService, a service for calculating the salary of an employee
public class InputRankSalaryService : ISalaryService {
    public bool TryCalculateSalary(EmployeeType employeeType, int SalaryInputRank, out decimal result) {
        // since we don't really have strong typed enums in C#, this can actually throw an exception, if someone does this;
        // EmployeeType t = (EmployeeType)1337;
        // t switch { ... }
        if (SalaryInputRank < 1 || SalaryInputRank > 10) {
            result = 0;
            return false;
        }
        var coefficient = employeeType switch {
            EmployeeType.Employee => 1.125,
            EmployeeType.Manager => 1.725,
            EmployeeType.CEO => 2.725,
            _ => throw new ArgumentException("Unhandled salary coefficient", nameof(employeeType))
        };
        result = Convert.ToDecimal(coefficient * SalaryInputRank);
        return true;
    }
}