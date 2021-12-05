using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using webapp.mvc.Models.ViewModels;
#pragma warning disable
namespace webapp.mvc.DataAccessLayer {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<LibraryItem> libraryItems { get; set; }
        public DbSet<Category> categoryItems { get; set; }

        public DbSet<Employee> employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<LibraryItem>().ToTable("LibraryItem");
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Employee>().ToTable("Employee");
        }
    }

    public static class LibraryContextExtension {
        public static async Task<EditLibraryItemModel?> EditLibraryModel(this ApplicationDbContext context, int id) {
            var categories = await context.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            var it = await context.libraryItems.FirstAsync(i => i.ID == id);
            return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type, IsBorrowable = it.IsBorrowable, Borrower = it.Borrower, BorrowDate = it.BorrowDate };
        }

        public static async Task<EditLibraryItemModel?> CreateLibraryModel(this ApplicationDbContext context, int id) {
            var categories = await context.categoryItems.Select(c => new SelectListItem { Value = c.ID.ToString(), Text = c.CategoryName }).ToListAsync();
            var it = await context.libraryItems.FirstAsync(i => i.ID == id);
            if (it == null) return null;
            return new EditLibraryItemModel { ID = it.ID, Title = it.Title, Author = it.Author, CategoryID = it.CategoryID, Categories = categories, Length = (it.RunTimeMinutes ?? it.Pages) ?? 0, Type = it.Type };
        }
    }
}
#pragma warning restore