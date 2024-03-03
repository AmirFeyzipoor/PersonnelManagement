using Microsoft.Extensions.Configuration;

namespace PersonnelManagement.IntegrationTest.Infrastructure
{
    public class ConfigurationFixture
    {
        public TestSettings Value { get; private set; }

        public ConfigurationFixture()
        {
            Value = GetSettings();
        }

        private TestSettings GetSettings()
        {
            var configurations = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(
                "appsettings.json", 
                optional: true, 
                reloadOnChange: false)
                .AddEnvironmentVariables()
                .AddCommandLine(Environment.GetCommandLineArgs())
                .Build();

            var settings = new TestSettings();
            configurations.Bind(settings);
            return settings;
        }
    }

    public class TestSettings
    {
        public string ConnectionString { get; set; }
    }

    [CollectionDefinition(
        nameof(ConfigurationFixture), 
        DisableParallelization = false)]
    public class ConfigurationCollectionFixture : 
        ICollectionFixture<ConfigurationFixture>
    {
    }
}
