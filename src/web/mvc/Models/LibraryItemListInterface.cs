using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.mvc.Models;

public abstract class ILibraryItem {
    public abstract string Listing { get; }
    public abstract string GetTitle();
}
