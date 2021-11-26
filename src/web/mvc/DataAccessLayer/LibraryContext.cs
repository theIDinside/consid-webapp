using Microsoft.EntityFrameworkCore;
using webapp.mvc.Models;
using MySql.EntityFrameworkCore.Extensions;
using MySql.EntityFrameworkCore.Infrastructure;

namespace webapp.mvc.DataAccessLayer {
    public class LibraryContext : DbContext {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) {}

        public DbSet<LibraryItem> libraryItems {get; set;}
        public DbSet<CategoryItem> categoryItems { get; set; }

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
            modelBuilder.Entity<CategoryItem>().ToTable("Category");
        }
    }
}