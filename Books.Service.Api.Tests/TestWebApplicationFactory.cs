using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Books.Service.Api.Tests
{
	public class TestWebApplicationFactory: WebApplicationFactory<Program>
	{
		public IServiceCollection ServiceCollection { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton(ConfigurationHelper.Config as IConfiguration);
                ServiceCollection = services;
            });
        }
    }
}

