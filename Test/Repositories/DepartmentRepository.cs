using Dapper;
using Test.DB.Models;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;
using Test1.DB.Models;

namespace Test.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DepartmentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            const string sql = @"
                SELECT d.Id, d.Name, d.Phone, d.CompanyId,
                       c.Id, c.Name
                FROM Department d
                JOIN Company c ON c.Id = d.CompanyId
                WHERE d.Id = @Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();

            var result = await conn.QueryAsync<Department, Company, Department>(
                sql,
                (department, company) =>
                {
                    department.Company = company;
                    return department;
                },
                new { Id = id },
                splitOn: "Id"
            );

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            const string sql = @"
                SELECT d.Id, d.Name, d.Phone, d.CompanyId,
                       c.Id, c.Name
                FROM Department d
                JOIN Company c ON c.Id = d.CompanyId;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();

            var result = await conn.QueryAsync<Department, Company, Department>(
                sql,
                (department, company) =>
                {
                    department.Company = company;
                    return department;
                },
                splitOn: "Id"
            );

            return result;
        }

        public async Task<int> AddAsync(Department department)
        {
            const string sql = @"
                INSERT INTO Department (Name, Phone, CompanyId)
                VALUES (@Name, @Phone, @CompanyId)
                RETURNING Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();

            return await conn.ExecuteScalarAsync<int>(sql, new
            {
                department.Name,
                department.Phone,
                department.CompanyId
            });
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            const string sql = @"
                UPDATE Department SET
                    Name = COALESCE(@Name, Name),
                    Phone = COALESCE(@Phone, Phone),
                    CompanyId = COALESCE(@CompanyId, CompanyId)
                WHERE Id = @Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();

            var rowsAffected = await conn.ExecuteAsync(sql, new
            {
                department.Name,
                department.Phone,
                department.CompanyId,
                department.Id
            });

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            const string sql = "DELETE FROM Department WHERE Id = @Id";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var rowsAffected = await conn.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
