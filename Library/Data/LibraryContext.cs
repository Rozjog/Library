using System;
using Library.entity;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    internal class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryDB;Trusted_Connection=True;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(bookEntity =>
            {
                bookEntity.HasKey(book => book.Id);
                bookEntity.Property(book => book.Title).IsRequired().HasMaxLength(200);
                bookEntity.Property(book => book.ISBN).HasMaxLength(20);       

                bookEntity.HasOne(book => book.Author)              
                    .WithMany(author => author.Books)
                    .HasForeignKey(book => book.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade); 

                bookEntity.HasOne(book => book.Genre)
                    .WithMany(genre => genre.Books)
                    .HasForeignKey(book => book.GenreId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Author>(authorEntity =>
            {
                authorEntity.HasKey(author => author.Id);
                authorEntity.Property(author => author.FirstName).IsRequired().HasMaxLength(50);
                authorEntity.Property(author => author.LastName).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Genre>(genreEntity =>
            {
                genreEntity.HasKey(genre => genre.Id);
                genreEntity.Property(genre => genre.Name).IsRequired().HasMaxLength(50);        
                genreEntity.Property(genre => genre.Description).HasMaxLength(500);
            });
        }
    }
}