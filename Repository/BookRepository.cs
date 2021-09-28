using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet5.Service.Exp.Models.Data;
using Amazon.Runtime;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Documents;
using Amazon.DynamoDBv2.DocumentModel;

namespace DotNet5.Service.Exp.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AwsDbConfig _localDbConfig;
        private readonly AmazonDynamoDBConfig _AmazonDynamoDBConfig;
        private readonly AmazonDynamoDBClient _AmazonDynamoDBClient;
        private readonly DynamoDBContext _DynamoDBContext;
        private readonly AwsCredentials _AwsCredentials;

        public BookRepository(AwsDbConfig localDbConfig)
        {
            _localDbConfig = localDbConfig;
            
            _AmazonDynamoDBConfig = new AmazonDynamoDBConfig();

            //_AmazonDynamoDBConfig.RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_localDbConfig.Region);
            _AmazonDynamoDBConfig.ServiceURL = _localDbConfig.ServiceURL;
            _AwsCredentials = new AwsCredentials(_localDbConfig);

            _AmazonDynamoDBClient = new AmazonDynamoDBClient(_AwsCredentials, _AmazonDynamoDBConfig);

            _DynamoDBContext = new DynamoDBContext(_AmazonDynamoDBClient);
        }

        public async Task<IEnumerable<Book>> SearchBooks(string title, int? ISBN)
        {
            var conditions = new List<ScanCondition>();

            if(!string.IsNullOrWhiteSpace(title))
            {
                conditions.Add(new ScanCondition ("Title", ScanOperator.Equal, new []{ title }));
            }
            return await _DynamoDBContext.ScanAsync<Book>(conditions, null).GetRemainingAsync();
        }

        public async Task<Book> GetBookAsync(int bookId)
        {
            return await _DynamoDBContext.LoadAsync<Book>(bookId);
        }


        public async Task AddBookAsync(Book book)
        {
            await _DynamoDBContext.SaveAsync(book);
        }

        public async Task CreateTableAsync()
        {
            await _AmazonDynamoDBClient.CreateTableAsync("Book", new List<Amazon.DynamoDBv2.Model.KeySchemaElement> {
                new Amazon.DynamoDBv2.Model.KeySchemaElement { AttributeName = "BookId", KeyType = "HASH" }
                }
                , new List<Amazon.DynamoDBv2.Model.AttributeDefinition> { new Amazon.DynamoDBv2.Model.AttributeDefinition { AttributeName = "BookId", AttributeType = "N" } }
                , new Amazon.DynamoDBv2.Model.ProvisionedThroughput { ReadCapacityUnits = 1, WriteCapacityUnits = 1 });
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            //var response = _AmazonDynamoDBClient.ScanAsync("Book", new List<string> { "BookId", "ISBN", "Title" }).Result;

            var scanConfig = new ScanOperationConfig();

            var table = _DynamoDBContext.GetTargetTable<Book>();

            var results = table.Scan(scanConfig);

            List<Amazon.DynamoDBv2.DocumentModel.Document> data = await results.GetRemainingAsync();

            IEnumerable<Book> books = _DynamoDBContext.FromDocuments<Book>(data);

            return books;
        }

        public async Task DeleteBookAsync(int bookId)
        {
            await _DynamoDBContext.DeleteAsync<Book>(bookId);
        }
    }

    public interface IBookRepository
    {
        Task<IEnumerable<Book>> SearchBooks(string title, int? ISBN);
        Task<Book> GetBookAsync(int bookId);
        Task DeleteBookAsync(int bookId);
        Task CreateTableAsync();
        Task AddBookAsync(Book book);
        Task<IEnumerable<Book>> GetAllBooksAsync();
    }
}
