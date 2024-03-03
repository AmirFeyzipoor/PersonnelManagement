using PersonnelManagement.Infrastructure.Data;

namespace PersonnelManagement.IntegrationTest.Infrastructure
{
    [Collection(nameof(ConfigurationFixture))]
    public class EFDataContextDatabaseFixture : DatabaseFixture
    {
        public readonly ConfigurationFixture _configuration;

        public EFDataContextDatabaseFixture(ConfigurationFixture configuration)
        {
            _configuration = configuration;
        }

        public DapperDataContext CreateDapperContext()
        {
            return new DapperDataContext(_configuration.Value.ConnectionString);
        }
        
        public DataContext CreateDataContext()
        {
            return new DataContext(_configuration.Value.ConnectionString);
        }
    }
}
