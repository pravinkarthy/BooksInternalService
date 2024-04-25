using System;
using Microsoft.Extensions.Configuration;

namespace Books.Service.Api.Tests
{
	public class ConfigurationHelper
	{
		static ConfigurationHelper()
		{
			var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

			Config = config.AddEnvironmentVariables().Build();
		}
		public static IConfigurationRoot Config;
	}
}

