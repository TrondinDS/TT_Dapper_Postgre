using Npgsql;
using System.Data;

namespace Test1.DB.DbConnectionFactory
{
    public class NpgsqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public NpgsqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException("Connection string is missing");
        }

        public async Task<IDbConnection> CreateOpenConnectionAsync()
        {
            var connection = new Npgsql.NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }
    }

}
