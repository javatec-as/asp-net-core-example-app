using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using WebApplication;

namespace Webapp.Tests
{
    public abstract class AbstractTest
    {
        protected IHost Host;

        [SetUp]
        public async Task Setup()
        {
            Host = CreateHostBuilder(new string[] { })
                .Build();

            (await Host
                    .InsertMockData())
                .InitNhibernateSessionContext();

            await Host.StartAsync();
        }

        [TearDown]
        public void Cleanup()
        {
            Host.Dispose();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseTestServer();
                });
    }
}