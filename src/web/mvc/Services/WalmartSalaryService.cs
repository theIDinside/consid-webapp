using webapp.mvc.Models;
namespace webapp.mvc.Services;

// This is what's nice with Services. Dependong on what we've registered, D.I. handles stuff for us.
public class WalmarSalaryService : ISalaryService {
    public bool TryCalculateSalary(EmployeeType employeeType, int SalaryInputRank, out Decimal Salary) {
        if (SalaryInputRank < 1 || SalaryInputRank > 10) {
            Salary = 0;
            return false;
        }
        var coefficient = employeeType switch {
            EmployeeType.Employee => 1.125,
            EmployeeType.Manager => 1.725,
            EmployeeType.CEO => 1078,
            _ => throw new ArgumentException("Unhandled salary coefficient", nameof(employeeType))
        };
        Salary = Convert.ToDecimal(coefficient * SalaryInputRank);
        return true;
    }
}

public static class WalmartSalaryServiceExtension {
    public static void AddInputRankService(this IServiceCollection collection) {
        collection.AddSingleton(new InputRankSalaryService());
    }
}