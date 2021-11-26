using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace webapp.mvc.DataAccessLayer {
    using Models;

    public static class SeedDB {
        public static void Initialize(IServiceProvider provider) {
            using(var ctx = new LibraryContext(provider.GetRequiredService<DbContextOptions<LibraryContext>>())) {
                // Database should be seeded by the createtables.sql, but if not;
                if(ctx.libraryItems.Any()) return;
                ctx.categoryItems.AddRange(
                    	new CategoryItem{CategoryName = "Computer Science"},
	                    new CategoryItem{CategoryName = "Art"},
	                    new CategoryItem{CategoryName = "Programming"},
	                    new CategoryItem{CategoryName = "Horror"},
	                    new CategoryItem{CategoryName = "Drama"}
                    );
                ctx.SaveChanges();
                var catCSID = ctx.categoryItems.Where(item => item.CategoryName == "Computer Science").Select(s => s.ID).First();
                var catHorrorID = ctx.categoryItems.Where(item => item.CategoryName == "Horror").Select(s => s.ID).First();
                var catProgrammingID = ctx.categoryItems.Where(item => item.CategoryName == "Programming").Select(s => s.ID).First();

                ctx.libraryItems.AddRange(
	                new LibraryItem{CategoryID = catProgrammingID, Title= "The C++ Programming Language",Author="Bjarne Strostroup",Pages= 1376, IsBorrowable= true, Type ="book"},
	                new LibraryItem{CategoryID = catProgrammingID, Title= "Effective C++",Author="Scott Meyers",Pages= 297,IsBorrowable=true, Type ="book"},
	                new LibraryItem{CategoryID = catCSID,Title= "Computer Security, 3rd Edition",Author="Dieter Gollman",Pages= 436,IsBorrowable= true, Type ="book"},
                    new LibraryItem{CategoryID = catCSID,Title= "Computer Architecture: A Quantitative Approach",Author="David A. Patterson",Pages= 856,IsBorrowable= true, Type ="book"},
	                new LibraryItem{CategoryID = catCSID,Title= "Mikroprocessorteknik",Author="Per Foyer",Pages= 276, IsBorrowable= true, Type ="book"},
	                new LibraryItem{CategoryID = catProgrammingID, Title= "The Linux Programming Interface",Author="Michael Kerrisk",Pages= 1506,IsBorrowable= true, Type ="book"},
                    new LibraryItem {CategoryID = catCSID,      Title="Intel 64 and IA-32 architectures software developer's manual volume 1: Basic architecture", Author="Intel", Pages=482, IsBorrowable=false, Type="reference book"},
                    new LibraryItem {CategoryID = catCSID,      Title="Intel 64 and IA-32 architectures optimization reference manual", Author="Intel", Pages=868, IsBorrowable=false, Type="reference book"},
                    new LibraryItem {CategoryID = catHorrorID,  Title="Event Horizon",Author= "Philip Eisner",RunTimeMinutes=96, IsBorrowable=true, Type="dvd"},
                    new LibraryItem {CategoryID = catHorrorID,  Title="Alien: Covenant",Author= "Dan O'Bannon",RunTimeMinutes=122, IsBorrowable=true, Type="dvd"},
                    new LibraryItem {CategoryID = catHorrorID,  Title="Predators",Author="Alex Litvak",RunTimeMinutes=107,IsBorrowable=true, Type="dvd"},
                    new LibraryItem {CategoryID = catHorrorID,  Title="Pontypool",Author="Tony Burgess",RunTimeMinutes= 93,IsBorrowable=true, Type="dvd"}
                );
                ctx.SaveChanges();
                ctx.libraryItems.Add(new LibraryItem{CategoryID = catProgrammingID, Title="Foo bar", Author="Baz", Pages = 1337, IsBorrowable = false, Type = "reference book"});
                ctx.SaveChanges();
            }
        }
    }
}