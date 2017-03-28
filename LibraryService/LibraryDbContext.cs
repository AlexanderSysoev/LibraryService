using System.Data.Entity;

namespace LibraryService
{
    public class LibraryDbContext : DbContext
    {
        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Author> Authors { get; set; }
    }
}
