using Microsoft.EntityFrameworkCore;
using Library.entity;

namespace Library.Data
{
    internal class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(bookEntity =>
            {
                bookEntity.HasKey(book => book.Id);

                bookEntity.Property(book => book.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                bookEntity.Property(book => book.ISBN)
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Author>(authorEntity =>
            {
                authorEntity.HasKey(author => author.Id);

                authorEntity.Property(author => author.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                authorEntity.Property(author => author.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                authorEntity.Property(author => author.Country)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Genre>(genreEntity =>
            {
                genreEntity.HasKey(genre => genre.Id);

                genreEntity.Property(genre => genre.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                genreEntity.Property(genre => genre.Description)
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<BookGenre>(entity =>
            {
                entity.HasKey(bg => new { bg.BookId, bg.GenreId });

                entity.HasOne(bg => bg.Book)
                    .WithMany(b => b.BookGenres)
                    .HasForeignKey(bg => bg.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(bg => bg.Genre)
                    .WithMany(g => g.BookGenres)
                    .HasForeignKey(bg => bg.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BookAuthor>(entity =>
            {
                entity.HasKey(ba => new { ba.BookId, ba.AuthorId });

                entity.HasOne(ba => ba.Book)
                    .WithMany(b => b.BookAuthors)
                    .HasForeignKey(ba => ba.BookId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ba => ba.Author)
                    .WithMany(a => a.BookAuthors)
                    .HasForeignKey(ba => ba.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}