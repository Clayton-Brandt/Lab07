using ClaytonBlazorApp.Models;
using System.Collections.Generic;

namespace ClaytonBlazorApp.Services
{
    public interface ILibraryService
    {
        List<Book> GetBooks();
        List<User> GetUsers();
        void AddBook(Book book);
        void EditBook(Book book);
        void DeleteBook(int id);
        void AddUser(User user);
        void EditUser(User user);
        void DeleteUser(int id);
        void BorrowBook(int userId, int bookId);
        void ReturnBook(int userId, int bookIndex);
        Dictionary<int, List<Book>> GetBorrowedBooks(); // key is userId now
    }
}