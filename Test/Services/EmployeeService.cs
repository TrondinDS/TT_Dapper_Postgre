using AutoMapper;
using Test.DTO.Employe;
using Test.Repositories.Interfaces;
using Test.Services.Interfaces;
using Test1.DB.Models;

namespace Test.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<int> AddEmployeeAsync(EmployeeCreateDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            return await _employeeRepository.AddEmployeeAsync(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            return await _employeeRepository.DeleteEmployeeAsync(employeeId);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyIdAsync(int companyId)
        {
            var employees = await _employeeRepository.GetEmployeesByCompanyIdAsync(companyId);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyNameAsync(string companyName)
        {
            var employees = await _employeeRepository.GetEmployeesByCompanyNameAsync(companyName);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentId);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string departmentName)
        {
            var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentName);
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee == null ? null : _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateDto employeeUpdateDto)
        {
            var employee = _mapper.Map<Employee>(employeeUpdateDto);
            return await _employeeRepository.UpdateEmployeeAsync(employee);
        }
    }
}
