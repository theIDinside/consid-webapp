using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace webapp.mvc.DataAccessLayer {
    using Models;
    using webapp.mvc.Services;

    public static class SeedDB {


        public static void Initialize(IServiceProvider provider) {
            using (var ctx = new LibraryContext(provider.GetRequiredService<DbContextOptions<LibraryContext>>())) {
                // Database should be seeded by the createtables.sql, but if not;
                ctx.Database.EnsureCreated();
                if (ctx.libraryItems.Any()) return;
                ctx.categoryItems.AddRange(
                        new Category { CategoryName = "Computer Science" },
                        new Category { CategoryName = "Art" },
                        new Category { CategoryName = "Programming" },
                        new Category { CategoryName = "Horror" },
                        new Category { CategoryName = "Drama" }
                    );
                ctx.SaveChanges();
                var catCSID = ctx.categoryItems.Where(item => item.CategoryName == "Computer Science").Select(s => s.ID).First();
                var catHorrorID = ctx.categoryItems.Where(item => item.CategoryName == "Horror").Select(s => s.ID).First();
                var catProgrammingID = ctx.categoryItems.Where(item => item.CategoryName == "Programming").Select(s => s.ID).First();

                ctx.libraryItems.AddRange(
                    new LibraryItem { CategoryID = catProgrammingID, Title = "The C++ Programming Language", Author = "Bjarne Strostroup", Pages = 1376, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "Effective C++", Author = "Scott Meyers", Pages = 297, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catCSID, Title = "Computer Security, 3rd Edition", Author = "Dieter Gollman", Pages = 436, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catCSID, Title = "Computer Architecture: A Quantitative Approach", Author = "David A. Patterson", Pages = 856, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catCSID, Title = "Mikroprocessorteknik", Author = "Per Foyer", Pages = 276, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "The Linux Programming Interface", Author = "Michael Kerrisk", Pages = 1506, IsBorrowable = true, Borrower = "", Type = "book" },
                    new LibraryItem { CategoryID = catCSID, Title = "Intel 64 and IA-32 architectures software developer's manual volume 1: Basic architecture", Author = "Intel", Pages = 482, IsBorrowable = false, Borrower = "", Type = "reference book" },
                    new LibraryItem { CategoryID = catCSID, Title = "Intel 64 and IA-32 architectures optimization reference manual", Author = "Intel", Pages = 868, IsBorrowable = false, Borrower = "", Type = "reference book" },
                    new LibraryItem { CategoryID = catHorrorID, Title = "Event Horizon", Author = "Philip Eisner", RunTimeMinutes = 96, IsBorrowable = true, Borrower = "", Type = "dvd" },
                    new LibraryItem { CategoryID = catHorrorID, Title = "Alien: Covenant", Author = "Dan O'Bannon", RunTimeMinutes = 122, IsBorrowable = true, Borrower = "", Type = "dvd" },
                    new LibraryItem { CategoryID = catHorrorID, Title = "Predators", Author = "Alex Litvak", RunTimeMinutes = 107, IsBorrowable = true, Borrower = "", Type = "dvd" },
                    new LibraryItem { CategoryID = catHorrorID, Title = "Pontypool", Author = "Tony Burgess", RunTimeMinutes = 93, IsBorrowable = true, Borrower = "", Type = "dvd" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "Performance Analysis and Tuning on Modern CPUS", Author = "Denis Bakhvalov", Pages = 175, IsBorrowable = false, Borrower = "", Type = "reference book" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "C++ Data Structures And Algorithm Design Principles", Author = "John Carey", Pages = 597, IsBorrowable = false, Borrower = "", Type = "reference book" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "Modern X86 Assembly Language Programming 2nd Edition", Author = "Daniel Kusswurm", Pages = 604, IsBorrowable = false, Borrower = "", Type = "reference book" },
                    new LibraryItem { CategoryID = catProgrammingID, Title = "Foo bar", Author = "Baz", Pages = 1337, IsBorrowable = false, Borrower = "", Type = "reference book" }
                );
                ctx.SaveChanges();

                ctx.SaveChanges();
            }
        }
    }
}