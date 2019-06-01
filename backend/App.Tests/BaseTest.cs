using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Core;
using Serilog.Events;

namespace AppTests
{
    public class BaseTest<T> where T: class
    {
        protected Logger logger;
        protected ILoggerFactory loggerFactory;
        protected IConfiguration configuration;

        protected BaseTest()
        {

        }

        [SetUp]
        public void Init()
        {
            var logFilename = Path.Combine("Logs", NUnit.Framework.TestContext.CurrentContext.Test.FullName + ".log");
            DeleteLogFile(logFilename);
            logger = CreateLogger(logFilename);
            loggerFactory = new SerilogLoggerFactory(logger, false);
            configuration = LoadConfig();
        }

        private IConfiguration LoadConfig()
        {
            return new ConfigurationBuilder()
              .AddUserSecrets<T>()
              .AddEnvironmentVariables()
              .Build();
        }

        private Logger CreateLogger(string filename)
        {
            return new LoggerConfiguration()
              .MinimumLevel.Debug()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
              .Enrich.FromLogContext()
              .WriteTo.File(filename)
              .CreateLogger();
        }

        private void DeleteLogFile(string filename)
        {
            File.Delete(filename);
        }
    }
}