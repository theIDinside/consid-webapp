using Microsoft.Extensions.Options;
using webapp.mvc.Models;
namespace webapp.mvc.Services {

    public enum SalaryServiceTypes {
        InputRank,
        Walmart
    }

    public class SalaryServiceOptions {
        public virtual string ServiceName { get; set; }
    }

    // Rank Salary calculation service
    public interface ISalaryService {
        Decimal CalculateSalary(EmployeeType employeeType, int SalaryInput);
    }
}