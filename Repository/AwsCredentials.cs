using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNet5.Service.Exp.Repository
{
    public class AwsCredentials : Amazon.Runtime.AWSCredentials
    {
		private readonly AwsDbConfig _AwsDbConfig;

		public AwsCredentials(AwsDbConfig config)
		{
			_AwsDbConfig = config;
		}

		public override Amazon.Runtime.ImmutableCredentials GetCredentials()
		{
			return new Amazon.Runtime.ImmutableCredentials(_AwsDbConfig.AwsAccessKey, _AwsDbConfig.AwsSecretKey, null);
		}
	}

}
