using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.mvc.Models;

public class LibraryItemListViewModel {
    public IEnumerable<webapp.mvc.Models.LibraryItem> ModelData { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}