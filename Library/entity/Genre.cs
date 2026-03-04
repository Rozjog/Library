using System.Collections.Generic;

namespace Library.entity
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();
    }
}