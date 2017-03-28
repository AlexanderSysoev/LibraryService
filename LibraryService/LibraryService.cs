using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace LibraryService
{
    public class LibraryService
    {
        private readonly LibraryDbContext _dbContext;

        public LibraryService(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Book Find(IEnumerable<string> authors)
        {
            return
                _dbContext.Books.Include("Authors")
                    .FirstOrDefault(b => b.Authors.Any(a => authors.Contains(a.LastName)));
        }

        public BookStatus GetStatus(Book book)
        {
            var dbBook = GetById(book.Id);
            return dbBook.Status;
        }

        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Senior)]
        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Middle)]
        public void Get(Book book)
        {
            var dbBook = GetById(book.Id);

            dbBook.Status = BookStatus.InUse;
            _dbContext.SaveChanges();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Senior)]
        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Middle)]
        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Junior)]
        public void Return(Book book)
        {
            var dbBook = GetById(book.Id);

            dbBook.Status = BookStatus.Free;
            _dbContext.SaveChanges();
        }

        [PrincipalPermission(SecurityAction.Demand, Role = LibrarianRoles.Senior)]
        public void Recycle(Book book)
        {
            var dbBook = GetById(book.Id);

            dbBook.Status = BookStatus.Recycled;
            _dbContext.SaveChanges();
        }

        private Book GetById(int id)
        {
            var dbBook = _dbContext.Books.FirstOrDefault(b => b.Id == id);
            if (dbBook == null)
            {
                throw new ApplicationException($"Book with Id={id} not found");
            }

            return dbBook;
        }
    }
}
