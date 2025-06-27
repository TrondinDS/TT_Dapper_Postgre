using Dapper;
using Microsoft.AspNetCore.Connections;
using Test.DB.Models;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;

namespace Test.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CompanyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, Name FROM Company WHERE Id = @Id";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.QuerySingleOrDefaultAsync<Company>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            const string sql = "SELECT Id, Name FROM Company";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.QueryAsync<Company>(sql);
        }

        public async Task<int> AddAsync(Company company)
        {
            const string sql = @"
                INSERT INTO Company (Name)
                VALUES (@Name)
                RETURNING Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            return await conn.ExecuteScalarAsync<int>(sql, new { company.Name });
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            const string sql = @"
                UPDATE Company
                SET Name = COALESCE(@Name, Name)
                WHERE Id = @Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var affectedRows = await conn.ExecuteAsync(sql, new { company.Name, company.Id });
            return affectedRows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Company WHERE Id = @Id";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var affectedRows = await conn.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}
