using System;
using NLog;
using NUnit.Framework;

namespace Twidlle.Infrastructure.Testing
{
    public class BaseSetUpFixture
    {
        [OneTimeSetUp]
        public virtual void OneTimeSetUp() 
            => _logger.Info("Test run starting...");

        [OneTimeTearDown]
        public virtual void OneTimeTearDown() 
            => _logger.Info($"Test run finished.{Environment.NewLine}");

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    }
}
