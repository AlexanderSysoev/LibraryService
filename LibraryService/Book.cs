using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryService
{
    public class Book
    {
        public Book()
        {
            Authors = new HashSet<Author>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Author> Authors { get; set; }

        public BookStatus Status { get; set; }
    }
}
