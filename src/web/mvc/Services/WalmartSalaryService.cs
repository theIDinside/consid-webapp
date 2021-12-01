using webapp.mvc.Models;
namespace webapp.mvc.Services;

// This is what's nice with Services. Dependong on what we've registered, D.I. handles stuff for us.
public class WalmarSalaryService : ISalaryService {
    public decimal CalculateSalary(EmployeeType employeeType, int SalaryInputRank) {
        var coefficient = employeeType switch {
            EmployeeType.Employee => 1.125,
            EmployeeType.Manager => 1.725,
            EmployeeType.CEO => 1078,
            _ => throw new ArgumentException("Unhandled salary coefficient", nameof(employeeType))
        };
        return Convert.ToDecimal(coefficient * SalaryInputRank);
    }
}

public static class WalmartSalaryServiceExtension {
    public static void AddInputRankService(this IServiceCollection collection) {
        collection.AddSingleton(new InputRankSalaryService());
    }
}