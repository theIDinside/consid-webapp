using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable
namespace webapp.mvc.Models {
    [Table("LibraryItem")]
    public class LibraryItem {
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }

        [Column("CategoryID")]
        [Display(Name = "Category")]
        [Required]
        [ForeignKey("CategoryID")]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Author { get; set; }

        [Column("Pages")]
        public int? Pages { get; set; }

        [Column("RunTimeMinutes")]
        public int? RunTimeMinutes { get; set; }

        [Column("IsBorrowable")]
        public bool IsBorrowable { get; set; }

        [Column("Borrower")]
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(100)]
        public string Borrower { get; set; }

        // we have to add the 'TypeName = "Date"', because otherwise, Entity Framework will create a DATETIME type, which is not what we want.

        [Display(Name = "Borrow date")]
        [DataType(DataType.Date)]
        [Column("BorrowDate", TypeName = "Date")]
        public DateTime? BorrowDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        // What E.F. calls a navigation property; essentially what the foreign key points to. We can say .Include(i => i.Category), and E.F. will hook it up for us, behind the scenes.
        public virtual Category Category { get; set; }

        [NotMapped]
        public string displayDate {
            get {
                return BorrowDate?.ToString("yyyy-MM-dd") ?? "";
            }
        }
    }

    public static class DisplayListingExtension {
        public static string ListDisplay(this string Title) {
            // This method gets called when we want to display this library item's name, concatenated with the abbreviation
            var abbreviation = string.Join("",
                    Title.Split(' ')
                    .Where(substr => substr.Length > 0 && char.IsLetterOrDigit(substr[0]))
                    .Select(substr => char.ToUpper(substr[0]))
                    .ToArray()
                );
            return $"{Title} ({abbreviation})";
        }

    }
}
#pragma warning restore