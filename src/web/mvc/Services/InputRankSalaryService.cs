using webapp.mvc.Models;
namespace webapp.mvc.Services;

public class InputRankSalaryService : ISalaryService {
    public decimal CalculateSalary(EmployeeType employeeType, int SalaryInputRank) {
        var coefficient = employeeType switch {
            EmployeeType.Employee => 1.125,
            EmployeeType.Manager => 1.725,
            EmployeeType.CEO => 2.725,
            _ => throw new ArgumentException("Unhandled salary coefficient", nameof(employeeType))
        };
        return Convert.ToDecimal(coefficient * SalaryInputRank);
    }
}