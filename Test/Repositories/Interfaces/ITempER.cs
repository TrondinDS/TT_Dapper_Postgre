using System.Data;
using Test.Repositories.Generic;
using Test1.DB.Models;

namespace Test.Repositories.Interfaces
{
    public interface ITempER
    {
        Task<int> AddAsync(Employee employee, IDbConnection conn, IDbTransaction trx);
        Task<bool> EnsureCompanyExistsAsync(int companyId, IDbConnection conn, IDbTransaction trx);
        Task<bool> EnsureDepartmentExistsAsync(int departmentId, IDbConnection conn, IDbTransaction trx);
        Task<bool> EnsurePassportUniqueAsync(Employee employee, IDbConnection conn, IDbTransaction trx);

        Task<int?> DeleteAsync(int Id, IDbConnection conn, IDbTransaction trx);

        Task UpdateAsync(Employee employee, IDbConnection conn, IDbTransaction trx);

        Task<IEnumerable<Employee>> GetAllAsync(IDbConnection conn, IDbTransaction trx);
        Task<Employee> GetByIdAsync(int Id, IDbConnection conn, IDbTransaction trx);

        Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId, IDbConnection conn, IDbTransaction trx);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyNameAsync(string companyName, IDbConnection conn, IDbTransaction trx);

        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId, IDbConnection conn, IDbTransaction trx);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string departmentName, IDbConnection conn, IDbTransaction trx);

        Task<DbTransactionContext> BeginTransactionAsync();

    }
}
