using Microsoft.AspNetCore.Mvc.Rendering;
namespace webapp.mvc.Models.ViewModels;

public class EditLibraryItemModel : CreateLibraryItemModel {
    public int ID { get; set; }
    public bool? IsBorrowable { get; set; }
    public string? Borrower { get; set; }
    public DateTime? BorrowDate { get; set; }

    public string? ViewTab { get; set; }
}