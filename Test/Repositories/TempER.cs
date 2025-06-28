using Dapper;
using System.Data;
using System.Threading.Tasks;
using Test.DB.Models;
using Test.Repositories.Generic;
using Test.Repositories.Interfaces;
using Test1.DB.DbConnectionFactory;
using Test1.DB.Models;

namespace Test.Repositories
{
    public class TempER : GenericRepository<Employee>, ITempER
    {
        public TempER(IDbConnectionFactory factory) : base(factory) { }

        protected override string TableName => "Employee";
        protected override string KeyName => "Id";

        // Методы проверки для добавления:
        public async Task<bool> EnsureCompanyExistsAsync(int companyId, IDbConnection conn, IDbTransaction trx)
        {
            const string checkCompanySql = "SELECT * FROM Company WHERE Id = @Id";
            return await conn.ExecuteScalarAsync<bool>(checkCompanySql, new { Id = companyId }, trx);
        }

        public async Task<bool> EnsureDepartmentExistsAsync(int departmentId, IDbConnection conn, IDbTransaction trx)
        {
            const string checkDepartmentSql = "SELECT * FROM Department WHERE Id = @Id";
            return await conn.ExecuteScalarAsync<bool>(checkDepartmentSql, new { Id = departmentId }, trx);
        }

        public async Task<bool> EnsurePassportUniqueAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
        {
            const string findPassportSql = @"
                SELECT EmployeeId FROM Passport
                WHERE Number = @Number AND Type = @Type;
            ";

            var existingPassport = await conn.ExecuteScalarAsync<int?>(
                    findPassportSql,
                    new { employee.Passport.Number, employee.Passport.Type },
                    trx
            );

            if (existingPassport.HasValue)
                return false;

            return true;
        }

        public async Task<bool> EnsurePhoneUniqueAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
        {
            const string findPhoneSql = @"
                SELECT Id FROM Employee
                WHERE Phone = @Phone;
            ";

            var existingEmployeeId = await conn.ExecuteScalarAsync<int?>(
                findPhoneSql,
                new { employee.Phone },
                trx
            );

            return !existingEmployeeId.HasValue;
        }

        public async Task<bool> EnsurePassportUniqueForUpdateAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
        {
            const string findPassportSql = @"
                SELECT EmployeeId FROM Passport
                WHERE Number = @Number AND Type = @Type;
            ";

            var existingEmployeeId = await conn.ExecuteScalarAsync<int?>(
                findPassportSql,
                new { employee.Passport.Number, employee.Passport.Type },
                trx
            );

            // Если ничего не найдено паспорт уникален
            if (!existingEmployeeId.HasValue)
                return true;

            // Если паспорт текущего сотрудника то нормас
            return existingEmployeeId.Value == employee.Id;
        }

        public async Task<bool> EnsurePhoneUniqueForUpdateAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
        {
            const string findPhoneSql = @"
                SELECT Id FROM Employee
                WHERE Phone = @Phone;
            ";

            var existingEmployeeId = await conn.ExecuteScalarAsync<int?>(
                findPhoneSql,
                new { employee.Phone },
                trx
            );

            // Если такого телефона нет — значит уникальный
            if (!existingEmployeeId.HasValue)
                return true;

            // Если найденный номер принадлежит текущему сотруднику — не конфликтует
            return existingEmployeeId.Value == employee.Id;
        }

        // Метод добавления:
        public async Task<int> AddAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
        {
            const string insertEmployeeSql = @"
                INSERT INTO Employee (Name, Surname, Phone, CompanyId, DepartmentId)
                VALUES (@Name, @Surname, @Phone, @CompanyId, @DepartmentId)
                RETURNING Id;
            ";

            const string insertPassportSql = @"
                INSERT INTO Passport (EmployeeId, Type, Number)
                VALUES (@EmployeeId, @Type, @Number);
            ";

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

            return employeeId;
        }

        // Метод удаления:
        public async Task<int?> DeleteAsync(int Id, IDbConnection conn, IDbTransaction trx)
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

            return await conn.ExecuteScalarAsync<int?>(sql, new { Id }, trx);
        }

        // Метод обновления:
        public async Task UpdateAsync(Employee employee, IDbConnection conn, IDbTransaction trx)
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
        }

        // Метод возвращения всех:
        public new async Task<IEnumerable<Employee>> GetAllAsync(IDbConnection conn, IDbTransaction trx)
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

            return await QueryEmployeesWithCompanyAsync(sql, parameters: null, conn, trx);
        }

        // Метод возвращения по Id:
        public new async Task<Employee> GetByIdAsync(int Id, IDbConnection conn, IDbTransaction trx)
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
                WHERE e.Id = @Id;
            ";

            var result = await QueryEmployeesWithCompanyAsync(sql, new { Id }, conn, trx);
            return result.FirstOrDefault();
        }

        // Метод возвращения работников компании по Id департамента:
        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId, IDbConnection conn, IDbTransaction trx)
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

            return await QueryEmployeesWithCompanyAsync(sql, new { CompanyId = companyId }, conn, trx);
        }

        // Метод возвращения работников компании по Названию компании:
        public async Task<IEnumerable<Employee>> GetEmployeesByCompanyNameAsync(string companyName, IDbConnection conn, IDbTransaction trx)
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
            }, conn, trx);
        }

        // Метод возвращения работников департамента по Id департамента:
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId, IDbConnection conn, IDbTransaction trx)
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

            return await QueryEmployeesWithCompanyAsync(sql, new { DepartmentId = departmentId }, conn, trx);
        }

        // Метод возвращения работников департамента по Названию департамента:
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string departmentName, IDbConnection conn, IDbTransaction trx)
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
            }, conn, trx);
        }

        // Метод вызова получений:
        private async Task<IEnumerable<Employee>> QueryEmployeesWithCompanyAsync(
            string sql,
            object parameters,
            IDbConnection conn,
            IDbTransaction trx)
        {
            var result = await conn.QueryAsync<Employee, Passport, Department, Company, Employee>(
                sql,
                (e, p, d, c) =>
                {
                    e.Passport = p;
                    e.Department = d;
                    e.Company = c;
                    return e;
                },
                param: parameters,
                transaction: trx,
                splitOn: "PassportEmployeeId,Id,Id"
            );

            return result;
        }


        private new async Task<int> AddAsync(Employee entity)
        {
            return await base.AddAsync(entity);
        }
        private new async Task<bool> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        private new async Task<bool> UpdateAsync(Employee entity)
        {
            return await base.UpdateAsync(entity);
        }
    }
}
