using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using DotNet5.Service.Exp.Domain;
using DotNet5.Service.Exp.Models.Data;
using DotNet5.Service.Exp.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DotNet5.Service.Exp
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly AwsDbConfig _localDbConfig;
        private readonly IBookRepository _bookRepo;
        private readonly IBookStore _bookStore;

        public Worker(ILogger<Worker> logger, IConfiguration config, IBookStore bookstore)
        {
            _logger = logger;
            _config = config;

            _localDbConfig = new AwsDbConfig(_config);
            _bookRepo = new BookRepository(_localDbConfig);
            _bookStore = bookstore;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // lets make sure theres no books in DynamoDb
            await _bookStore.DeleteAllBooks(stoppingToken);
            // lets a few books
            await _bookStore.CreateRandomBooks(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);

                // get a list of books
                var books = _bookStore.GetAllBooks().Result.ToArray();

                // lets search for the first book
                if (books != null && books.Any())
                {
                    var searchResult = _bookStore.SearchBooksByTitle(books[0].Title).Result;

                    if (searchResult != null && searchResult.Any())
                    {
                        // lets delete the book we found
                        foreach (var book in searchResult)
                        {
                            await _bookStore.DeleteBookByIdAsync(book.BookId);
                        }
                    }
                }
            }
        }

        public override Task StopAsync(System.Threading.CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker - StopAsync received");

            return base.StopAsync(stoppingToken);
        }

        public override Task StartAsync(System.Threading.CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Worker - StartAsync received");

            return base.StartAsync(stoppingToken);
        }
    }
}
