using Dapper;
using System.Data;
using Test.DB.Models;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;
using Test1.DB.Models;

namespace Test1.DB.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmployeeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            const string findPassportSql = @"
                SELECT EmployeeId FROM Passport
                WHERE Number = @Number AND Type = @Type;
            ";

            const string insertEmployeeSql = @"
                INSERT INTO Employee (Name, Surname, Phone, CompanyId, DepartmentId)
                VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId)
                RETURNING Id;
            ";

            const string insertPassportSql = @"
                INSERT INTO Passport (EmployeeId, Type, Number)
                VALUES (@EmployeeId, @Type, @Number);
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            using var trx = conn.BeginTransaction();

            try
            {
                if(employee.CompanyId == null)
                    throw new InvalidOperationException("Отсутвует id компания.");

                await EnsureCompanyExistsAsync((int)employee.CompanyId, conn, trx);

                if (employee.DepartmentId == null)
                    throw new InvalidOperationException("Отсутвует id депортамента.");

                await EnsureDepartmentExistsAsync((int)employee.DepartmentId, conn, trx);

                var employeeId = await conn.ExecuteScalarAsync<int>(
                    insertEmployeeSql,
                    new
                    {
                        employee.Name,
                        employee.Surname,
                        employee.Phone,
                        employee.CompanyId,
                        employee.DepartmentId
                    },
                    trx
                );

                var existingPassport = await conn.ExecuteScalarAsync<int?>(
                    findPassportSql,
                    new { employee.Passport.Number, employee.Passport.Type },
                    trx
                );

                if (existingPassport.HasValue)
                    throw new InvalidOperationException("Такой паспорт уже зарегистрирован.");

                await conn.ExecuteAsync(
                    insertPassportSql,
                    new
                    {
                        EmployeeId = employeeId,
                        employee.Passport.Type,
                        employee.Passport.Number
                    },
                    trx
                );

                trx.Commit();
                return employeeId;
            }
            catch
            {
                trx.Rollback();
                throw;
            }
        }

        private async Task EnsureCompanyExistsAsync(int companyId, IDbConnection conn, IDbTransaction trx)
        {
            const string checkCompanySql = "SELECT * FROM Company WHERE Id = @Id";

            var exists = await conn.ExecuteScalarAsync<bool>(checkCompanySql, new { Id = companyId }, trx);
            if (!exists)
                throw new InvalidOperationException($"Компания с Id = {companyId} не найдена.");
        }

        private async Task EnsureDepartmentExistsAsync(int departmentId, IDbConnection conn, IDbTransaction trx)
        {
            const string checkDepartmentSql = "SELECT * FROM Department WHERE Id = @Id";

            var exists = await conn.ExecuteScalarAsync<bool>(checkDepartmentSql, new { Id = departmentId }, trx);
            if (!exists)
                throw new InvalidOperationException($"Отдел с Id = {departmentId} не найден.");
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            using var trx = conn.BeginTransaction();

            try
            {
                var sql = @"
                    WITH deleted_employee AS (
                        DELETE FROM Employee
                        WHERE Id = @Id
                        RETURNING Id, DepartmentId
                    ),
                    deleted_passport AS (
                        DELETE FROM Passport WHERE EmployeeId = @Id
                    )
                    SELECT DepartmentId FROM deleted_employee;
                ";

                var departmentId = await conn.ExecuteScalarAsync<int?>(sql, new { Id = employeeId }, trx);
                if (departmentId == null)
                {
                    trx.Rollback();
                    return false;
                }

                // Убраны за ненадобностью
                //var count = await conn.ExecuteScalarAsync<int>(
                //    "SELECT COUNT(*) FROM Employee WHERE DepartmentId = @DepartmentId",
                //    new { DepartmentId = departmentId.Value },
                //    trx);

                //if (count == 0)
                //{
                //    await conn.ExecuteAsync(
                //        "DELETE FROM Department WHERE Id = @DepartmentId",
                //        new { DepartmentId = departmentId.Value },
                //        trx);
                //}

                trx.Commit();
                return true;
            }
            catch
            {
                trx.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId)
        {
            var sql = @"
                SELECT e.*, 
                       p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId
                WHERE e.CompanyId = @CompanyId;
            ";

            return await QueryEmployeesWithCompanyAsync(sql, new { CompanyId = companyId });
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyNameAsync(string companyName)
        {
            var sql = @"
                SELECT e.*, 
                       p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId
                WHERE c.Name ILIKE @CompanyName;
            ";

            return await QueryEmployeesWithCompanyAsync(sql, new
            {
                CompanyName = $"%{companyName}%"
            });
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var sql = @"
                SELECT e.*, 
                       p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId
                WHERE e.DepartmentId = @DepartmentId;
            ";

            return await QueryEmployeesWithCompanyAsync(sql, new { DepartmentId = departmentId });
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string departmentName)
        {
            var sql = @"
                SELECT e.*, 
                       p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId
                WHERE d.Name ILIKE @DepartmentName;
            ";

            return await QueryEmployeesWithCompanyAsync(sql, new
            {
                DepartmentName = $"%{departmentName}%"
            });
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var sql = @"
                SELECT e.*, p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId
                WHERE e.Id = @Id;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var result = await conn.QueryAsync<Employee, Passport, Department, Company, Employee>(
                sql,
                (e, p, d, c) =>
                {
                    e.Passport = p;
                    e.Department = d;
                    e.Company = c;
                    return e;
                },
                new { Id = id },
                splitOn: "PassportEmployeeId,Id,Id"
            );

            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            var sql = @"
                SELECT e.*, 
                       p.EmployeeId AS PassportEmployeeId, p.Type, p.Number,
                       d.Id, d.Name, d.Phone,
                       c.Id, c.Name
                FROM Employee e
                JOIN Passport p ON p.EmployeeId = e.Id
                JOIN Department d ON d.Id = e.DepartmentId
                JOIN Company c ON c.Id = e.CompanyId;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var result = await conn.QueryAsync<Employee, Passport, Department, Company, Employee>(
                sql,
                (e, p, d, c) =>
                {
                    e.Passport = p;
                    e.Department = d;
                    e.Company = c;
                    return e;
                },
                splitOn: "PassportEmployeeId,Id,Id"
            );

            return result;
        }

        private async Task<IEnumerable<Employee>> QueryEmployeesWithCompanyAsync(
            string sql,
            object parameters)
        {
            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            var result = await conn.QueryAsync<Employee, Passport, Department, Company, Employee>(
                sql,
                (e, p, d, c) =>
                {
                    e.Passport = p;
                    e.Department = d;
                    e.Company = c;
                    return e;
                },
                parameters,
                splitOn: "PassportEmployeeId,Id,Id"
            );

            return result;
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            const string updateEmployeeSql = @"
                UPDATE Employee SET
                    Name = COALESCE(@Name, Name),
                    Surname = COALESCE(@Surname, Surname),
                    Phone = COALESCE(@Phone, Phone),
                    CompanyId = COALESCE(@CompanyId, CompanyId),
                    DepartmentId = COALESCE(@DepartmentId, DepartmentId)
                WHERE Id = @Id;
            ";

            const string updatePassportSql = @"
                UPDATE Passport SET
                    Type = COALESCE(@Type, Type),
                    Number = COALESCE(@Number, Number)
                WHERE EmployeeId = @EmployeeId;
            ";

            using var conn = await _connectionFactory.CreateOpenConnectionAsync();
            using var trx = conn.BeginTransaction();

            try
            {
                await conn.ExecuteAsync(updateEmployeeSql, new
                {
                    employee.Name,
                    employee.Surname,
                    employee.Phone,
                    employee.CompanyId,
                    employee.DepartmentId,
                    employee.Id
                }, trx);

                if (employee.Passport is not null)
                {
                    await conn.ExecuteAsync(updatePassportSql, new
                    {
                        employee.Passport.Type,
                        employee.Passport.Number,
                        EmployeeId = employee.Id
                    }, trx);
                }

                trx.Commit();
                return true;
            }
            catch
            {
                trx.Rollback();
                return false;
            }
        }
    }
}
