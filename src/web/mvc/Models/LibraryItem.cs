using System.ComponentModel.DataAnnotations;

namespace webapp.mvc.Models {
    public class LibraryItem {
        public int ID { get; set; }
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? Pages { get; set; }
        public int? RunTimeMinutes { get; set; }
        public Boolean IsBorrowable { get; set; }
        public string Borrower { get; set; }
        public DateTime? BorrowDate { get; set; }
        public string Type { get; set; }
    }
}