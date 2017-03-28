using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryService
{
    public class Author
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        [Index]
        [MaxLength(100)]
        public string LastName { get; set; }
    }
}
