using ClaytonBlazorApp.Models;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ClaytonBlazorApp.Services
{
    public class LibraryService : ILibraryService
    {
        private List<Book> books = new();
        private List<User> users = new();
        private Dictionary<int, List<Book>> borrowedBooks = new();

        private string booksFile;
        private string usersFile;

        private bool isTestMode;

        // Constructor used by the Blazor app, sets file paths and loads data from CSV files.
        public LibraryService(IWebHostEnvironment env)
        {
            isTestMode = false;

            booksFile = Path.Combine(env.ContentRootPath, "Data", "Books.csv");
            usersFile = Path.Combine(env.ContentRootPath, "Data", "Users.csv");

            ReadBooks();
            ReadUsers();
        }

        // Constructor used for unit tests, avoids file access and uses in-memory lists only.
        public LibraryService()
        {
            isTestMode = true;

            books = new List<Book>();
            users = new List<User>();
            borrowedBooks = new Dictionary<int, List<Book>>();

            booksFile = string.Empty;
            usersFile = string.Empty;
        }

        // Reads books from CSV file into the books list.
        private void ReadBooks()
        {
            if (isTestMode) return;
            if (!File.Exists(booksFile)) return;

            books.Clear();

            foreach (var line in File.ReadLines(booksFile))
            {
                var fields = line.Split(',');
                if (fields.Length >= 4)
                {
                    books.Add(new Book
                    {
                        Id = int.Parse(fields[0].Trim()),
                        Title = fields[1].Trim(),
                        Author = fields[2].Trim(),
                        ISBN = fields[3].Trim()
                    });
                }
            }
        }

        // Reads users from CSV file into the users list.
        private void ReadUsers()
        {
            if (isTestMode) return;
            if (!File.Exists(usersFile)) return;

            users.Clear();

            foreach (var line in File.ReadLines(usersFile))
            {
                var fields = line.Split(',');
                if (fields.Length >= 3)
                {
                    users.Add(new User
                    {
                        Id = int.Parse(fields[0].Trim()),
                        Name = fields[1].Trim(),
                        Email = fields[2].Trim()
                    });
                }
            }
        }

        // Writes current books list back to CSV file.
        private void WriteBooks()
        {
            if (isTestMode) return;

            var lines = books.Select(b => $"{b.Id},{b.Title},{b.Author},{b.ISBN}");
            File.WriteAllLines(booksFile, lines);
        }

        // Writes current users list back to CSV file.
        private void WriteUsers()
        {
            if (isTestMode) return;

            var lines = users.Select(u => $"{u.Id},{u.Name},{u.Email}");
            File.WriteAllLines(usersFile, lines);
        }

        // Returns a copy of the books list.
        public List<Book> GetBooks() => new List<Book>(books);

        // Returns a copy of the users list.
        public List<User> GetUsers() => new List<User>(users);

        // Returns the dictionary of borrowed books.
        public Dictionary<int, List<Book>> GetBorrowedBooks() => borrowedBooks;

        // Adds a new book and assigns an Id.
        public void AddBook(Book book)
        {
            book.Id = books.Any() ? books.Max(b => b.Id) + 1 : 1;
            books.Add(book);
            WriteBooks();
        }

        // Updates an existing book.
        public void EditBook(Book updated)
        {
            var book = books.FirstOrDefault(b => b.Id == updated.Id);
            if (book == null) return;

            book.Title = updated.Title;
            book.Author = updated.Author;
            book.ISBN = updated.ISBN;

            WriteBooks();
        }

        // Deletes a book by Id.
        public void DeleteBook(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null) return;

            books.Remove(book);
            WriteBooks();
        }

        // Adds a new user and assigns an Id.
        public void AddUser(User user)
        {
            user.Id = users.Any() ? users.Max(u => u.Id) + 1 : 1;
            users.Add(user);
            WriteUsers();
        }

        // Updates an existing user.
        public void EditUser(User updated)
        {
            var user = users.FirstOrDefault(u => u.Id == updated.Id);
            if (user == null) return;

            user.Name = updated.Name;
            user.Email = updated.Email;

            WriteUsers();
        }

        // Deletes a user and removes their borrowed books if they exist.
        public void DeleteUser(int id)
        {
            var user = users.FirstOrDefault(u => u.Id == id);
            if (user == null) return;

            users.Remove(user);
            WriteUsers();

            if (borrowedBooks.ContainsKey(id))
                borrowedBooks.Remove(id);
        }

        // Borrows a book and moves it into the borrowed dictionary.
        public void BorrowBook(int userId, int bookId)
        {
            var user = users.FirstOrDefault(u => u.Id == userId);
            var book = books.FirstOrDefault(b => b.Id == bookId);

            if (user == null || book == null) return;

            if (!borrowedBooks.ContainsKey(userId))
                borrowedBooks[userId] = new List<Book>();

            borrowedBooks[userId].Add(book);
            books.Remove(book);

            WriteBooks();
        }

        // Returns a borrowed book back to the main list.
        public void ReturnBook(int userId, int bookIndex)
        {
            if (!borrowedBooks.ContainsKey(userId)) return;

            var book = borrowedBooks[userId][bookIndex];
            borrowedBooks[userId].RemoveAt(bookIndex);
            books.Add(book);

            WriteBooks();
        }
    }
}