using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace DotNet5.Service.Exp.Models.Data
{
    [DynamoDBTable("Book")]
    public class Book
    {
        [DynamoDBHashKey]
        public int BookId { get; set; }

        public string Title { get; set; }
        public int ISBN { get; set; }

        [DynamoDBProperty("Authors")]
        public List<string> BookAuthors { get; set; }

        [DynamoDBIgnore]
        public string CoverPage { get; set; }
    }
}
