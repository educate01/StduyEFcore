using studyEFcore.Models;
using Microsoft.EntityFrameworkCore;

namespace studyEFcore.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Book> Books => Set<Book>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Category constraints
            modelBuilder.Entity<Category>(b =>
            {
                b.HasKey(c => c.CategoryId);
                b.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });

            // Author constraints
            modelBuilder.Entity<Author>(b =>
            {
                b.HasKey(a => a.AuthorId);
                b.Property(a => a.Name).IsRequired().HasMaxLength(100);
                b.Property(a => a.Country).HasMaxLength(100);
            });

            // Book constraints + relation to Author
            modelBuilder.Entity<Book>(b =>
            {
                b.HasKey(x => x.BookId);
                b.Property(x => x.Title).IsRequired().HasMaxLength(200);
                b.Property(x => x.Year);
                b.HasOne(x => x.Author)
                 .WithMany(a => a.Books)
                 .HasForeignKey(x => x.AuthorId)
                 .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(x => x.Category)
                 .WithMany()
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.SetNull);
            });

            // --- Seeding data (HasData) ---
            // Seed Authors and Books (minst 2 authors, minst 3 böcker)
            modelBuilder.Entity<Author>().HasData(
                new Author { AuthorId = 1, Name = "Erik Eriksson", Country = "Sweden" },
                new Author { AuthorId = 2, Name = "Anna Andersson", Country = "Sweden" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { BookId = 1, Title = "Svenska äventyr", Year = 2010, AuthorId = 1 },
                new Book { BookId = 2, Title = "Databaser för nybörjare", Year = 2020, AuthorId = 2 },
                new Book { BookId = 3, Title = "Fördjupning i EF Core", Year = 2022, AuthorId = 1 }
            );

            // Optional: seed Categories if du vill ha initiala kategorier
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, Name = "Skönlitteratur" },
                new Category { CategoryId = 2, Name = "Facklitteratur" }
            );
        }

        // För enkelhet: överrida OnConfiguring (om du inte konfigurerar via DI)
        // Men vi kommer konfigurera i Program.cs med options.
    }
}
