using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.entity
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PublishYear { get; set; }
        public string ISBN { get; set; }
        public int QuantityInStock { get; set; }
        public int AuthorId { get; set; }
        public int GenreId { get; set; }
        public Author Author { get; set; }
        public Genre Genre { get; set; }
    }
}
