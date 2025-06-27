using Test1.DB.Models;

namespace Test.Repositories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int> AddEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyIdAsync(int companyId);
        Task<IEnumerable<Employee>> GetEmployeesByCompanyNameAsync(string companyName);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(string departmentName);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<bool> UpdateEmployeeAsync(Employee employee);
    }
}
