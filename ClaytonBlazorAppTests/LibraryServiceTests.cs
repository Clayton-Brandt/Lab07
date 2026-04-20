using Xunit;
using ClaytonBlazorApp.Services;
using ClaytonBlazorApp.Models;
using System.Linq;

namespace ClaytonBlazorAppTests
{
    public class LibraryServiceTests
    {
        // Tests that adding a book increases the total number of books.
        [Fact]
        public void AddBook_ShouldIncreaseCount()
        {
            var service = new LibraryService();

            service.AddBook(new Book { Title = "Test", Author = "A", ISBN = "123" });

            Assert.Single(service.GetBooks());
        }

        // Tests that a new book gets assigned an Id starting at 1.
        [Fact]
        public void AddBook_ShouldAssignId()
        {
            var service = new LibraryService();

            service.AddBook(new Book { Title = "Test" });

            Assert.Equal(1, service.GetBooks().First().Id);
        }

        // Tests that deleting a valid book removes it from the list.
        [Fact]
        public void DeleteBook_ValidId_ShouldRemoveBook()
        {
            var service = new LibraryService();

            service.AddBook(new Book { Title = "Test" });

            var id = service.GetBooks().First().Id;

            service.DeleteBook(id);

            Assert.Empty(service.GetBooks());
        }

        // Tests that deleting an invalid book id does nothing.
        [Fact]
        public void DeleteBook_InvalidId_ShouldDoNothing()
        {
            var service = new LibraryService();

            service.DeleteBook(999);

            Assert.Empty(service.GetBooks());
        }

        // Tests that borrowing a valid book moves it from books to borrowed books.
        [Fact]
        public void BorrowBook_ValidInput_ShouldMoveBook()
        {
            var service = new LibraryService();

            service.AddBook(new Book { Title = "Test" });
            service.AddUser(new User { Name = "John" });

            var bookId = service.GetBooks().First().Id;
            var userId = service.GetUsers().First().Id;

            service.BorrowBook(userId, bookId);

            Assert.Empty(service.GetBooks());
            Assert.Single(service.GetBorrowedBooks()[userId]);
        }

        // Tests that borrowing a non-existent book fails and does nothing.
        [Fact]
        public void BorrowBook_InvalidBook_ShouldFail()
        {
            var service = new LibraryService();

            service.AddUser(new User { Name = "John" });

            var userId = service.GetUsers().First().Id;

            service.BorrowBook(userId, 999);

            Assert.Empty(service.GetBorrowedBooks());
        }

        // Tests that returning a borrowed book moves it back to the main list.
        [Fact]
        public void ReturnBook_ShouldMoveBookBack()
        {
            var service = new LibraryService();

            service.AddBook(new Book { Title = "Test" });
            service.AddUser(new User { Name = "John" });

            var bookId = service.GetBooks().First().Id;
            var userId = service.GetUsers().First().Id;

            service.BorrowBook(userId, bookId);

            service.ReturnBook(userId, 0);

            Assert.Single(service.GetBooks());
        }

        // Tests that returning a book when none are borrowed does nothing.
        [Fact]
        public void ReturnBook_NoBooks_ShouldFail()
        {
            var service = new LibraryService();

            service.AddUser(new User { Name = "John" });

            var userId = service.GetUsers().First().Id;

            service.ReturnBook(userId, 0);

            Assert.Empty(service.GetBorrowedBooks());
        }
    }
}