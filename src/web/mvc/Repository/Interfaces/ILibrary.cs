using webapp.mvc.Models.ViewModels;
using webapp.mvc.Repository;

namespace mvc.Repository.Interfaces {
    // This interface, would basically make it possible to do testing, or just swap out "Library" backends. In production code
    // this interface would probably expose much more functionality
    public interface ILibrary {

        ILibraryItemRepository LibraryItems { get; set; }
        ICategoryRepository Categories { get; set; }
        int Commit();
        // commits all changes made to the backend, async
        Task<int> CommitAsync();
        // dispose of the context
        void Dispose();
        // helper method, that creats a view model, sort of like a " Edit-view-model" of the Library Item model
        Task<EditLibraryItemModel?> GetEditLibraryItemModel(int id);
        // helper method, that creats a view model, sort of like a "create view model" of the Library Item model
        Task<CreateLibraryItemModel> GetCreateLibraryItemModel();
    }
}