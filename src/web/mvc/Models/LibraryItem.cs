using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace webapp.mvc.Models
{
    public class LibraryItem
    {
        public int ID { get; set; }
        [Display(Name = "Category")]
        public int CategoryID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int? Pages { get; set; }
        [Display(Name = "Run time")]
        public int? RunTimeMinutes { get; set; }
        [Display(Name = "Availability")]
        public bool IsBorrowable { get; set; }
        public string Borrower { get; set; }
        [Display(Name = "Borrow date")]
        public DateTime? BorrowDate { get; set; }
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
        public virtual Category Category { get; set; }
        // I'm not really certain, that this is idiomatic Entity Framework code (I'm pretty sure it isn't); however, it is the solution I came up with.
        // And seeing as how I would be applying for a Junior developer position, I'm 100% certain, someone at Consid could enlighten me, how
        // this is supposed to work. This property, is what we set in the "Create" action of Library Item, and we then call into the DB and find an entry in the Category table matching that name
        // to get it's ID so that when we store a new Library Item, with the foreign key CategoryID set to that.
        [NotMapped]
        public string categoryNameSelect { get; set; }

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
                return BorrowDate?.ToString("yyyy-MM-dd");
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