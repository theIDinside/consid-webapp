using Microsoft.Extensions.Options;
using webapp.mvc.Models;
#pragma warning disable
namespace webapp.mvc.Services {
    public enum SalaryServiceTypes {
        InputRank,
        Walmart
    }

    public class SalaryServiceOptions {
        public string ServiceName { get; set; }
    }

    // Rank Salary calculation service
    public interface ISalaryService {
        Decimal CalculateSalary(EmployeeType employeeType, int SalaryInput);
    }
}
#pragma warning restore