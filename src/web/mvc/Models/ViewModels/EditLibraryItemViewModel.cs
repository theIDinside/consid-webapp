using Microsoft.AspNetCore.Mvc.Rendering;
namespace webapp.mvc.Models.ViewModels;

public class EditLibraryItemModel : CreateLibraryItemModel {
    public int ID { get; set; }

    public override LibraryItem? ToLibraryItem() {
        var item = base.ToLibraryItem();
        if (item != null) {
            ID = item.ID;
        }
        return item;
    }

    public bool? IsBorrowable { get; set; }
    public string? Borrower { get; set; }
    public DateTime? BorrowDate { get; set; }

    public string? ViewTab { get; set; }
}