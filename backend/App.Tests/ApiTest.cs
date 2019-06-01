using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using Serilog;

namespace AppTests
{
    public class ApiTest<T, S> : BaseTest<T> where T: class where S: class
    {
        private const int ApiTestRandomSeed = 42;
        protected HttpClient client;

        protected virtual void PreStartup()
        {

        }

        [SetUp]
        public new void Init()
        {
            PreStartup();
            Environment.SetEnvironmentVariable("Database__UseInMemory", bool.TrueString);
            Environment.SetEnvironmentVariable("App__RandomSeed", ApiTestRandomSeed.ToString());
            var factory = new WebApplicationFactory<S>();
            client = factory.WithWebHostBuilder(builder =>
            {
                builder.UseSerilog(logger);
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        protected async Task<HttpResponseMessage> RawGetAsync(string url)
        {
            return await client.GetAsync(url);
        }

        protected async Task RedirectGetAsync(string url, string expectedRedirectLocation)
        {
            var response = await client.GetAsync(url);
            Assert.AreEqual(HttpStatusCode.Redirect, response.StatusCode);
            var location = response.Headers.Location;
            Assert.AreEqual(expectedRedirectLocation, location);
        }

        protected async Task<V> GetAsync<V>(string url, HttpStatusCode expectedStatus = HttpStatusCode.OK)
        {
            var response = await client.GetAsync(url);
            Assert.AreEqual(expectedStatus, response.StatusCode);
            return await response.Content.ReadAsAsync<V>();
        }

        protected async Task DeleteAsync(string url, HttpStatusCode expectedStatus = HttpStatusCode.NoContent)
        {
            var response = await client.DeleteAsync(url);
            Assert.AreEqual(expectedStatus, response.StatusCode);
        }

        protected async Task<O> PostAsync<I, O>(string url, I input, HttpStatusCode expectedStatus = HttpStatusCode.OK)
        {
            var response = await client.PostAsJsonAsync<I>(url, input);
            Assert.AreEqual(expectedStatus, response.StatusCode);
            return await response.Content.ReadAsAsync<O>();
        }

        protected async Task<HttpResponseMessage> RawPostAsync(string url, Dictionary<string, string> formData)
        {
            var response = await client.PostAsync(url, new FormUrlEncodedContent(formData));
            return response;
        }
    }
}