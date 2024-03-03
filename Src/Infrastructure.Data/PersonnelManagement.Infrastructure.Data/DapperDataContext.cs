using System.Data;
using Microsoft.Data.SqlClient;

namespace PersonnelManagement.Infrastructure.Data;

public class DapperDataContext
{
    private readonly string _connectionString;

    public DapperDataContext(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public IDbConnection CreateConnection()
        => new SqlConnection(_connectionString);
}