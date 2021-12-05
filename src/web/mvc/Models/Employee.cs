using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.mvc.Models {
    public enum EmployeePosition {
        Employee = 1,
        Manager = 2,
        CEO = 3,
    }
    public class Employee {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "First name must be < 50 characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Last name must be < 50 characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        // Salary co-efficient
        public Decimal Salary { get; set; }

        [Display(Name = "Full name")]
        [NotMapped]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        public bool IsCEO { get; set; }

        public bool IsManager { get; set; }
        public int? ManagerID { get; set; }
    }
}
