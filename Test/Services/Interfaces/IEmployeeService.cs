using Test.DTO.Employe;
using Test1.DB.Models;

namespace Test.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<int> AddEmployeeAsync(EmployeeCreateDto employeeDto);
        Task<bool> DeleteEmployeeAsync(int employeeId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyIdAsync(int companyId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyNameAsync(string companyName);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string departmentName);
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<EmployeeDto?> GetByIdAsync(int id);
        Task<bool> UpdateEmployeeAsync(EmployeeUpdateDto employeeDto);
    }
}
