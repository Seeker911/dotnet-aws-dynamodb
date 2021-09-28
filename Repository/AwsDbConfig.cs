using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet5.Service.Exp.Repository
{
    public class AwsDbConfig
    {
        public AwsDbConfig(IConfiguration config)
        {
            config.GetSection("DynamoDb").Bind(this);
        }

        public string ProfileName { get; set; }
        public string Region { get; set; }
        public string ServiceURL { get; set; }

        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
    }
}
