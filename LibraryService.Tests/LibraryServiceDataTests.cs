using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;

namespace LibraryService.Tests
{
    [TestFixture]
    public class LibraryServiceDataTests
    {
        private LibraryDbContext _dbContext;
        private Book _book;

        [SetUp]
        public void Init()
        {
            TestHelpers.SetCurrentPrincipal(LibrarianRoles.Senior);
            _book = new Book
            {
                Name = "Энциклопедия путешествий",
                Authors = new Collection<Author>
                {
                    new Author
                    {
                        FirstName = "Андрей",
                        LastName = "Плешаков"
                    },
                    new Author
                    {
                        FirstName = "Степан",
                        LastName = "Глухов"
                    }
                },
                Status = BookStatus.InUse
            };

            _dbContext = new LibraryDbContext();
            _dbContext.Books.AddRange(new []{ _book });
            _dbContext.SaveChanges();
        }


        [Test]
        public void Find_FoundBookByAuthor()
        {
            var libraryService = new LibraryService(_dbContext);
            var book = libraryService.Find(new[] { "Глухов" });
            Assert.IsNotNull(book);
            Assert.Contains("Глухов", book.Authors.Select(a => a.LastName).ToList());
        }

        [Test]
        public void GetStatus_ReturnsStatus()
        {
            var libraryService = new LibraryService(_dbContext);
            var status = libraryService.GetStatus(_book);
            Assert.AreEqual(BookStatus.InUse, status);
        }

        [Test]
        public void Get_BookIsNotFree_ReturnsFalse()
        {
            var libraryService = new LibraryService(_dbContext);
            _book.Status = BookStatus.InUse;
            var result = libraryService.Get(_book);

            var book = _dbContext.Books.Find(_book.Id);
            Assert.IsFalse(result);
            Assert.AreEqual(BookStatus.InUse, book.Status);
        }

        [Test]
        public void Get_BookIsFree_SetStatusToInUseAndReturnsTrue()
        {
            var libraryService = new LibraryService(_dbContext);
            _book.Status = BookStatus.Free;
            var result = libraryService.Get(_book);

            var book = _dbContext.Books.Find(_book.Id);
            Assert.IsTrue(result);
            Assert.AreEqual(BookStatus.InUse, book.Status);
        }

        [Test]
        public void Return_SetStatusToFree()
        {
            var libraryService = new LibraryService(_dbContext);
            libraryService.Return(_book);

            var book = _dbContext.Books.Find(_book.Id);
            Assert.AreEqual(BookStatus.Free, book.Status);
        }

        [Test]
        public void Recycle_BookIsNotFree_ReturnsFalse()
        {
            var libraryService = new LibraryService(_dbContext);
            _book.Status = BookStatus.InUse;
            var result = libraryService.Recycle(_book);

            var book = _dbContext.Books.Find(_book.Id);
            Assert.AreEqual(BookStatus.InUse, book.Status);
            Assert.IsFalse(result);
        }

        [Test]
        public void Recycle_BookIsFree_SetStatusToRecycledAndReturnsTrue()
        {
            var libraryService = new LibraryService(_dbContext);
            _book.Status = BookStatus.Free;
            var result = libraryService.Recycle(_book);

            var book = _dbContext.Books.Find(_book.Id);
            Assert.AreEqual(BookStatus.Recycled, book.Status);
            Assert.IsTrue(result);
        }

        [TearDown]
        public void Clean()
        {
            _dbContext.Authors.RemoveRange(_book.Authors);
            _dbContext.Books.Remove(_book);
            _dbContext.SaveChanges();
            _dbContext.Dispose();
        }
    }
}
