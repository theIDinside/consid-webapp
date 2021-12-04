using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace webapp.mvc.Models.ViewModels;

public class CreateLibraryItemModel {

    [Required(ErrorMessage = "You must input a title")]
    public string Title { get; set; }
    [Required(ErrorMessage = "You must input an Author")]
    public string Author { get; set; }
    [Required]
    public int Length { get; set; }
    [Required(ErrorMessage = "You've altered the view model. You can only select from 4 categories")]
    public string Type { get; set; }
    [Required]
    public int CategoryID { get; set; }

    public List<SelectListItem> Categories { get; set; }

    public virtual LibraryItem? ToLibraryItem() {
        return Type switch {
            "book" => new LibraryItem { Title = Title, Author = Author, Type = Type, CategoryID = CategoryID, Pages = Length, Borrower = "", IsBorrowable = true },
            "reference book" => new LibraryItem { Title = Title, Author = Author, Type = Type, CategoryID = CategoryID, Pages = Length, Borrower = "", IsBorrowable = false },
            "audio book" or "dvd" => new LibraryItem { Title = Title, Author = Author, Type = Type, CategoryID = CategoryID, RunTimeMinutes = Length, Borrower = "", IsBorrowable = true },
            _ => null
        };
    }
}