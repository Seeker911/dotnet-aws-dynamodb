using DotNet5.Service.Exp.Models.Data;
using DotNet5.Service.Exp.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet5.Service.Exp.Domain
{
    public class BookStore : IBookStore
    {
        private readonly IBookRepository _repo;
        private readonly ILogger<BookStore> _logger;
        private readonly IConfiguration _config;

        public BookStore(IBookRepository repo, ILogger<BookStore> logger, IConfiguration config)
        {
            _repo = repo;
            _logger = logger;
            _config = config;
        }

        public Task<IEnumerable<Book>> SearchBooksByTitle(string title)
        {
            return _repo.SearchBooks(title, null);
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _repo.GetAllBooksAsync();
        }

        public async Task DeleteBookByIdAsync(int bookId)
        {
                Console.WriteLine($"Deleting book, BookId:[{bookId}]");

                await _repo.DeleteBookAsync(bookId);
        }

        public async Task DeleteAllBooks(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                var books = _repo.GetAllBooksAsync().Result;

                foreach (var book in books)
                {
                    Console.WriteLine($"Deleting book, BookId:[{book.BookId}], Title: [{book.Title}], ISBN: [{book.ISBN}]");

                    await _repo.DeleteBookAsync(book.BookId);
                }
            }
        }


        public async Task CreateRandomBooks(CancellationToken stoppingToken)
        {

            var books = new Book[] {
                new Book {
                    BookId = 1,
                    ISBN = 1234,
                    CoverPage = "cover page 1",
                    Title = "title 1",
                    BookAuthors = new List<string>{ "hans", "paul"}
                }

            , new Book {

                    BookId = 2,
                    ISBN = 5678,
                    CoverPage = "cover page 2",
                    Title = "title 2",
                    BookAuthors = new List<string>{ "nig", "nog"}
            }, new Book {
            BookId = 3,
                    ISBN = 1357,
                    CoverPage = "cover page 3",
                    Title = "title 3",
                    BookAuthors = new List<string>{ "pow", "wow"}
            }, new Book {
            BookId = 4,
                    ISBN = 2468,
                    CoverPage = "cover page 4",
                    Title = "title 4",
                    BookAuthors = new List<string>{ "not a", "hero"}
            } };

            var persisted = 0;

            while (!stoppingToken.IsCancellationRequested && persisted < books.Length)
            {
                try
                {
                    await _repo.AddBookAsync(books[persisted]);
                    _logger.LogInformation($"Added a book with BookId:[{books[persisted].BookId}] @ [{DateTimeOffset.Now}]");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    _logger.LogInformation("Failed to create a book with BookId[{i}] @ [{time}]", DateTimeOffset.Now);
                }
                persisted++;
            }
        }
    }

    public interface IBookStore
    {
        Task DeleteBookByIdAsync(int bookId);
        Task<IEnumerable<Book>> SearchBooksByTitle(string title);
        Task<IEnumerable<Book>> GetAllBooks();
        Task DeleteAllBooks(CancellationToken stoppingToken);
        Task CreateRandomBooks(CancellationToken stoppingToken);
    }
}
