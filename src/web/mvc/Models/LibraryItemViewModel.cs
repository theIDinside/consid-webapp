using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.mvc.Models;

public class LibraryItemListViewModel {
    public string Category;
    public string Title;
    public string Author;
    public string Length;
    public bool IsAvailable;
    public string Type;

}
