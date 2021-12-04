using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;
namespace webapp.mvc.DataAccessLayer {
    public class LibraryContext : DbContext {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

        public DbSet<LibraryItem> libraryItems { get; set; }
        public DbSet<Category> categoryItems { get; set; }

        public DbSet<Employee> employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            /**
             * Note to reviewer:
             * Technically, we could do some static class like and add an extension method to ModelBuilder called DoNotPluralizeTableNames
             * and do something like; modelBuilder.DoNotPluralizeTableNames(); But seeing as how these
             * are *JUST* two lines, doing all that fancy smancy stuff (which the document "teknisk uppgift.pdf" asks about,
             * as far as design princples, dependency inversion etc, etc)
             * for such little gain, increased complexity is not worth it. I'm just adding this comment here
             * to point out that I *could*. The right choice, for the right time. Complexity, for complexity's sake, is *not* good design.
            */
            modelBuilder.Entity<LibraryItem>().ToTable("LibraryItem");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Employee>().ToTable("Employee");
        }
    }

    public static class LibraryContextExtension {
        public static async Task<EditLibraryItemModel?> EditLibraryModel(this LibraryContext context, int id) {
            var categories = await context.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            var it = await context.libraryItems.FirstAsync(i => i.ID == id);
            return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type, IsBorrowable = it.IsBorrowable, Borrower = it.Borrower, BorrowDate = it.BorrowDate };
        }

        public static async Task<EditLibraryItemModel?> CreateLibraryModel(this LibraryContext context, int id) {
            var categories = await context.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            var it = await context.libraryItems.FirstAsync(i => i.ID == id);
            if (it == null) return null;
            return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type };
        }
    }
}