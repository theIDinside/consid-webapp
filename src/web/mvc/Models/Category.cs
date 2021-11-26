using System.Collections.Generic;

namespace webapp.mvc.Models
{
    public class Category
    {
        public int ID { get; set; }
        public string CategoryName { get; set; }

        // navigational property, i.e., all LibraryItems with "this" CategoryID
        public virtual ICollection<LibraryItem> LibraryItems { get; set; }

    }
}