using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using PersonnelManagement.Migrations;
using PersonnelManagement.RestApi.Configs.ServiceConfigs.ServicesPrerequisites;

namespace PersonnelManagement.RestApi.Configs.MigrationConfigs;

public static class MigrationConfig
{
    private static readonly ConnectionStrings _dbConnectionStrings = new();

    private static void Initialized(WebApplicationBuilder builder)
    {
        builder.Configuration.Bind("ConnectionStrings", _dbConnectionStrings);
    }

    public static void UpdateDataBases(this WebApplicationBuilder builder)
    {
        Initialized(builder);

        var connectionStrings = new List<string>
        {
            _dbConnectionStrings.SqlDb,
            _dbConnectionStrings.SqlTestDb
        };
        foreach (var connectionString in connectionStrings)
        {
            using var scope = ConfigureMigration(connectionString).CreateScope();

            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }

    private static IServiceProvider ConfigureMigration(string connectionString)
    {
        CreateDatabase(connectionString);

        var runner = CreateRunner(connectionString);

        return runner;
    }

    private static void CreateDatabase(string connectionString)
    {
        var databaseName = GetDatabaseName(connectionString);
        var masterConnectionString = ChangeDatabaseName(
            connectionString,
            "master");
        var commandScript = $"if db_id(N'{databaseName}') is null " +
                            $"create database [{databaseName}]";

        using var connection = new SqlConnection(masterConnectionString);
        using var command = new SqlCommand(commandScript, connection);
        connection.Open();
        command.ExecuteNonQuery();
        connection.Close();
    }

    private static string ChangeDatabaseName(
        string connectionString,
        string databaseName)
    {
        var csb = new SqlConnectionStringBuilder(connectionString)
        {
            InitialCatalog = databaseName
        };
        return csb.ConnectionString;
    }

    private static string GetDatabaseName(string connectionString)
    {
        return new SqlConnectionStringBuilder(connectionString).InitialCatalog;
    }

    private static IServiceProvider CreateRunner(
        string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(_ => _
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(ScriptResourceManager).Assembly).For.All())
            .AddSingleton<ScriptResourceManager>()
            .AddLogging(_ => _.AddFluentMigratorConsole())
            .BuildServiceProvider();
    }
}