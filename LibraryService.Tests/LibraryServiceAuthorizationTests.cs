using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security;
using Moq;
using NUnit.Framework;

namespace LibraryService.Tests
{
    [TestFixture]
    public class LibraryServiceAuthorizationTests
    {
        private LibraryService _libraryService;
        private Book _book;

        [SetUp]
        public void Init()
        {
            _book = new Book {Id = 1};
            var books = new List<Book>
            {
                _book
            }.AsQueryable();

            var mockBookSet = new Mock<DbSet<Book>>();
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Provider).Returns(books.Provider);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.Expression).Returns(books.Expression);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.ElementType).Returns(books.ElementType);
            mockBookSet.As<IQueryable<Book>>().Setup(m => m.GetEnumerator()).Returns(() => books.GetEnumerator());

            var mockAuthorsSet = new Mock<DbSet<Author>>();
            var mockContext = new Mock<LibraryDbContext>();
            mockContext.Setup(m => m.Books).Returns(mockBookSet.Object);
            mockContext.Setup(m => m.Authors).Returns(mockAuthorsSet.Object);

            _libraryService = new LibraryService(mockContext.Object);
        }

        [Test]
        public void JuniorLibrarianPermissionTests()
        {
            TestHelpers.SetCurrentPrincipal(LibrarianRoles.Junior);

            AssertNotAllowed(() => _libraryService.Get(_book));
            AssertAllowed(() => _libraryService.Return(_book));
            AssertNotAllowed(() => _libraryService.Recycle(_book));
        }

        [Test]
        public void MiddleLibrarianPermissionTests()
        {
            TestHelpers.SetCurrentPrincipal(LibrarianRoles.Middle);

            AssertAllowed(() => _libraryService.Get(_book));
            AssertAllowed(() => _libraryService.Return(_book));
            AssertNotAllowed(() => _libraryService.Recycle(_book));
        }

        [Test]
        public void SeniorLibrarianPermissionTests()
        {
            TestHelpers.SetCurrentPrincipal(LibrarianRoles.Senior);

            AssertAllowed(() => _libraryService.Get(_book));
            AssertAllowed(() => _libraryService.Return(_book));
            AssertAllowed(() => _libraryService.Recycle(_book));
        }

        private void AssertNotAllowed(TestDelegate del)
        {
            Assert.That(del, Throws.Exception.TypeOf<SecurityException>());
        }

        private void AssertAllowed(TestDelegate del)
        {
            try
            {
                del();
            }
            catch (SecurityException)
            {
                Assert.Fail("Expected no SecurityException");
            }
        }
    }
}
