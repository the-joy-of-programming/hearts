using System.IO;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Core;
using Serilog.Events;

namespace HeartsApp
{
    public class BaseTest
    {
        private Logger logger;
        protected ILoggerFactory loggerFactory;

        public BaseTest()
        {
            var logFilename = Path.Combine("Logs", NUnit.Framework.TestContext.CurrentContext.Test.FullName + ".log");
            DeleteLogFile(logFilename);
            logger = CreateLogger(logFilename);
            loggerFactory = new SerilogLoggerFactory(logger, false);
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