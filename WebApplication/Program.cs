using System;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NHibernate;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Nhibernate;

namespace WebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await (await CreateHostBuilder(args)
                .Build()
                .InsertMockData())
                .InitNhibernateSessionContext()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class ProgramExtensions
    {
        public static IHost InitNhibernateSessionContext(this IHost webHost)
        {
            AspNetCoreWebSessionContext.ContextAccessorInitializer = webHost.Services.GetService<Func<IHttpContextAccessor>>();
            return webHost;
        }

        public static async Task<IHost> InsertMockData(this IHost webHost)
        {
            // trigger db init by accessing ISessionFactory
            webHost.Services.GetService<ISessionFactory>();

            await using var connection = new SQLiteConnection(
                webHost.Services.GetService<IConfiguration>().GetConnectionString("DefaultConnection"));

            await connection.OpenAsync();

            foreach (var i in Enumerable.Range(1, 20))
            {
                await using var insertSql = new SQLiteCommand("INSERT INTO Customer (Name) VALUES (:name)", connection);
                insertSql.Parameters.Add(new SQLiteParameter(":name", $"Customer #{i}"));
                await insertSql.ExecuteNonQueryAsync();
            }

            foreach (var i in Enumerable.Range(1, 5))
            {
                await using var insertSql = new SQLiteCommand("INSERT INTO Contact (CustomerId, Name) VALUES (:customerId, :name)", connection);
                insertSql.Parameters.Add(new SQLiteParameter(":customerId", 1));
                insertSql.Parameters.Add(new SQLiteParameter(":name", $"Contact #{i}"));
                await insertSql.ExecuteNonQueryAsync();
            }

            return webHost;
        }
    }
}
