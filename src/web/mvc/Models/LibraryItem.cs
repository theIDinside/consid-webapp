using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace webapp.mvc.Models
{
    [Table("LibraryItem")]
    public class LibraryItem
    {
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int ID { get; set; }

        [Column("CategoryID")]
        [Display(Name = "Category")]
        [Required]
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
        [Required]
        public bool IsBorrowable { get; set; }

        [Column("Borrower")]
        [Required]
        [StringLength(100)]
        public string Borrower { get; set; }

        [Column("BorrowDate")]
        [Display(Name = "Borrow date")]
        [DataType(DataType.Date)]
        public DateTime? BorrowDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [NotMapped]
        [Display(Name = "Library Item")]
        public string Listing
        {
            get
            {
                // This method gets called when we want to display this library item's name, concatenated with the abbreviation
                var abbreviation = string.Join("",
                        this.Title.Split(' ')
                        .Where(substr => substr.Length > 0 && char.IsLetterOrDigit(substr[0]))
                        .Select(substr => char.ToUpper(substr[0]))
                        .ToArray()
                    );
                return $"{Title} ({abbreviation})";
            }
        }
        // What the "entity framework" calls a navigational property. It's essentially what CategoryID maps against, being a foreign key and all.
        // We use this navigation property, to display the name of the category this Library item is assigned to.
        // Caveat here; being this is day 2 of me using C# and ASP.NET, I'm not entirely sure this is idiomatic C#/EF, but it works.
        [ForeignKey("CategoryID")]
        public virtual Category Category { get; set; }

        [NotMapped]
        public int SelectedCategoryID;

        [NotMapped]
        public bool IsBorrowed
        {
            get
            {
                return BorrowDate.HasValue;
            }
        }
        [NotMapped]
        public string displayDate
        {
            get
            {
                return BorrowDate?.ToString("yyyy-MM-dd") ?? "";
            }
        }

        [NotMapped]
        public int LengthValue
        {
            get
            {
                if (Pages.HasValue) return Pages.Value;
                else if (RunTimeMinutes.HasValue) return RunTimeMinutes.Value;
                else return 0;
            }
        }
    }
}