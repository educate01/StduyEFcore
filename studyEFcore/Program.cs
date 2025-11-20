using Microsoft.EntityFrameworkCore;
using studyEFcore.Data;
using studyEFcore.Models;

var builder = new DbContextOptionsBuilder<AppDbContext>();
var dbPath = "Data Source=studyEFcore.db"; // fil i projektkatalog
builder.UseSqlite(dbPath);

using var context = new AppDbContext(builder.Options);

// Ensure database/migrations applied (we använder EF migrations i nästa steg).
// För lokal snabbtestning: uncomment nedan (men med migrations rekommenderas cmdline).
// context.Database.EnsureCreated();

Console.WriteLine("Bibliotek CLI");
Console.WriteLine("Skriv kommandon: categories | addcategory | updatecategory | deletecategory | authors | books | addauthor | addbook | booksbyauthor <id> | booksbyyear | searchbook | exit");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;
    var parts = input.Trim().Split(' ', 2);
    var cmd = parts[0].ToLowerInvariant();
    var arg = parts.Length > 1 ? parts[1] : null;

    switch (cmd)
    {
        case "exit":
            return;

        case "categories":
            var cats = await context.Categories.OrderBy(c => c.CategoryId).ToListAsync();
            Console.WriteLine("Categories:");
            foreach (var c in cats) Console.WriteLine($"{c.CategoryId} | {c.Name}");
            break;

        case "addcategory":
            Console.Write("Name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Ogiltigt namn."); break; }
            var newCat = new Category { Name = name! };
            context.Categories.Add(newCat);
            await context.SaveChangesAsync();
            Console.WriteLine($"Skapad Category {newCat.CategoryId}");
            break;

        case "updatecategory":
            Console.Write("CategoryId: ");
            if (!int.TryParse(Console.ReadLine(), out var upId)) { Console.WriteLine("Ogiltigt id"); break; }
            var upCat = await context.Categories.FindAsync(upId);
            if (upCat == null) { Console.WriteLine("Hittades inte."); break; }
            Console.Write($"Nytt namn (nu: {upCat.Name}): ");
            var newName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newName)) { Console.WriteLine("Ogiltigt namn."); break; }
            upCat.Name = newName!;
            await context.SaveChangesAsync();
            Console.WriteLine("Uppdaterad.");
            break;

        case "deletecategory":
            Console.Write("CategoryId: ");
            if (!int.TryParse(Console.ReadLine(), out var delId)) { Console.WriteLine("Ogiltigt id"); break; }
            var delCat = await context.Categories.FindAsync(delId);
            if (delCat == null) { Console.WriteLine("Hittades inte."); break; }
            context.Categories.Remove(delCat);
            await context.SaveChangesAsync();
            Console.WriteLine("Raderad.");
            break;

        case "authors":
            var authors = await context.Authors.OrderBy(a => a.AuthorId).ToListAsync();
            Console.WriteLine("Authors:");
            foreach (var a in authors) Console.WriteLine($"{a.AuthorId} | {a.Name} | {a.Country}");
            break;

        case "books":
            var books = await context.Books.Include(b => b.Author).OrderBy(b => b.BookId).ToListAsync();
            Console.WriteLine("Books:");
            foreach (var b in books) Console.WriteLine($"{b.BookId} | {b.Title} ({b.Year}) - {b.Author?.Name}");
            break;

        case "addauthor":
            Console.Write("Name: ");
            var aName = Console.ReadLine();
            Console.Write("Country: ");
            var aCountry = Console.ReadLine();
            var author = new Author { Name = aName ?? "Okänt", Country = aCountry };
            context.Authors.Add(author);
            await context.SaveChangesAsync();
            Console.WriteLine($"Skapad author {author.AuthorId}");
            break;

        case "addbook":
            // lista authors
            var auths = await context.Authors.OrderBy(a => a.AuthorId).ToListAsync();
            Console.WriteLine("Välj AuthorId:");
            foreach (var a in auths) Console.WriteLine($"{a.AuthorId} | {a.Name}");
            Console.Write("AuthorId: ");
            if (!int.TryParse(Console.ReadLine(), out var pickAuthor)) { Console.WriteLine("Ogiltigt id"); break; }
            Console.Write("Title: ");
            var t = Console.ReadLine();
            Console.Write("Year: ");
            if (!int.TryParse(Console.ReadLine(), out var y)) { Console.WriteLine("Ogiltigt år"); break; }

            var newBook = new Book { Title = t ?? "No title", Year = y, AuthorId = pickAuthor };
            context.Books.Add(newBook);
            await context.SaveChangesAsync();
            Console.WriteLine($"Skapad Book {newBook.BookId}");
            break;

        case "booksbyauthor":
            if (int.TryParse(arg ?? Console.ReadLine(), out var aid))
            {
                var bs = await context.Books.Where(b => b.AuthorId == aid).Include(b => b.Author).ToListAsync();
                foreach (var bk in bs) Console.WriteLine($"{bk.BookId} | {bk.Title} ({bk.Year}) - {bk.Author?.Name}");
            }
            else Console.WriteLine("Skriv ett giltigt authorId efter kommandot, t.ex. booksbyauthor 1");
            break;

        case "booksbyyear":
            var byYear = await context.Books.OrderBy(b => b.Year).Include(b => b.Author).ToListAsync();
            foreach (var bk in byYear) Console.WriteLine($"{bk.BookId} | {bk.Title} ({bk.Year}) - {bk.Author?.Name}");
            break;

        case "searchbook":
            Console.Write("Söktext i titel: ");
            var q = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(q)) { Console.WriteLine("Tom söksträng."); break; }
            var found = await context.Books
                .Where(b => b.Title.ToLower().Contains(q.ToLower()))
                .Include(b => b.Author)
                .ToListAsync();
            foreach (var bk in found) Console.WriteLine($"{bk.BookId} | {bk.Title} ({bk.Year}) - {bk.Author?.Name}");
            break;

        default:
            Console.WriteLine("Okänt kommando.");
            break;
    }
}
