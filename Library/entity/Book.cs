using System.Collections.Generic;

namespace Library.entity
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int PublishYear { get; set; }
        public string ISBN { get; set; } = "";
        public int QuantityInStock { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
        public string AuthorsDisplay
        {
            get
            {
                if (BookAuthors == null || BookAuthors.Count == 0)
                    return "";

                var authorNames = new List<string>();
                foreach (var ba in BookAuthors)
                {
                    if (ba.Author != null)
                        authorNames.Add($"{ba.Author.LastName} {ba.Author.FirstName}");
                }
                return string.Join(", ", authorNames);
            }
        }

        public string GenresDisplay
        {
            get
            {
                if (BookGenres == null || BookGenres.Count == 0)
                    return "";

                var genreNames = new List<string>();
                foreach (var bg in BookGenres)
                {
                    if (bg.Genre != null)
                        genreNames.Add(bg.Genre.Name);
                }
                return string.Join(", ", genreNames);
            }
        }
    }
}