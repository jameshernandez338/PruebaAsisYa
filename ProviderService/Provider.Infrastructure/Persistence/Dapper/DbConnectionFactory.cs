using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data.Common;

namespace Provider.Infrastructure.Persistence.Dapper;

public class DbConnectionFactory
{
    private readonly string _defaultConnectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _defaultConnectionString = configuration
            .GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string not configured.");
    }

    public DbConnection Create()
        => new SqlConnection(_defaultConnectionString);

    public DbConnection Create(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("La cadena de conexión no puede estar vacía", nameof(connectionString));

        return new SqlConnection(connectionString);
    }
}
