using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace poupeai_report_service.Tests.Helpers
{
    public class TestBase
    {
        protected Mock<IConfiguration> MockConfiguration { get; }
        protected Mock<ILogger> MockLogger { get; }

        public TestBase()
        {
            MockConfiguration = new Mock<IConfiguration>();
            MockLogger = new Mock<ILogger>();
        }

        protected static IConfiguration CreateMockConfiguration(Dictionary<string, string> settings)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(settings!)
                .Build();
            return configuration;
        }
    }
}