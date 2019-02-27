using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Serilog;

namespace HeartsApp
{
    public class ApiTest : BaseTest
    {
        private const int ApiTestRandomSeed = 42;
        protected HttpClient client;

        [SetUp]
        public new void Init()
        {
            Environment.SetEnvironmentVariable("Database__UseInMemory", bool.TrueString);
            Environment.SetEnvironmentVariable("App__RandomSeed", ApiTestRandomSeed.ToString());
            var factory = new WebApplicationFactory<Startup>();
            client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSerilog(logger);
            }).CreateClient();
        }

        protected async Task<T> GetAsync<T>(string url, HttpStatusCode expectedStatus = HttpStatusCode.OK)
        {
            var response = await client.GetAsync(url);
            Assert.AreEqual(expectedStatus, response.StatusCode);
            return await response.Content.ReadAsAsync<T>();
        }
    }
}