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
        /**
         * <summary> Tries to calculate salary based on the SalaryInput rank and stores the result in <paramref name="result">result</paramref></summary>
         * <returns>true if the calculation succeeds or false otherwise. Result is stored in the out parameter<paramref name="result">result</paramref></returns>
         */
        bool TryCalculateSalary(EmployeeType employeeType, int SalaryInputRank, out decimal result);
    }
}
#pragma warning restore