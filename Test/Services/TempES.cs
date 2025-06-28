using AutoMapper;
using Test.DTO.Employe;
using Test.Repositories.Interfaces;
using Test.Services.Interfaces;
using Test1.DB.Models;

namespace Test.Services
{
    public class TempES : IEmployeeService
    {
        private readonly ITempER _employeeRepository;
        private readonly IMapper _mapper;

        public TempES(ITempER employeeRepository, IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<int> AddEmployeeAsync(EmployeeCreateDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try 
            {
                if (employee.CompanyId == null) throw new InvalidOperationException("Отсутвует id компании.");
                if (employee.DepartmentId == null) throw new InvalidOperationException("Отсутвует id депортамента.");

                var exists = await _employeeRepository.EnsureCompanyExistsAsync(
                    (int)employee.CompanyId, 
                    trxContext.Connection, 
                    trxContext.Transaction
                );
                if (!exists) throw new InvalidOperationException($"Компания с Id = {employee.CompanyId} не найдена.");

                exists = await _employeeRepository.EnsureDepartmentExistsAsync(
                    (int)employee.DepartmentId, 
                    trxContext.Connection, 
                    trxContext.Transaction
                );
                if (!exists) throw new InvalidOperationException($"Отдел с Id = {employee.DepartmentId} не найден.");

                exists = await _employeeRepository.EnsurePassportUniqueAsync(
                    employee, 
                    trxContext.Connection, 
                    trxContext.Transaction
                );
                if (!exists) throw new InvalidOperationException("Такой паспорт уже зарегистрирован.");

                exists = await _employeeRepository.EnsurePhoneUniqueAsync(
                    employee,
                    trxContext.Connection,
                    trxContext.Transaction
                );
                if (!exists) throw new InvalidOperationException("Данный номер уже зарегистрирован.");

                var result = await _employeeRepository.AddAsync(
                    employee,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return result;

            }
            catch 
            {
                trxContext.Rollback();
                return 0;
            }

        }

        public async Task<bool> DeleteEmployeeAsync(int employeeId)
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var result = await _employeeRepository.DeleteAsync(
                    employeeId,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                if(result == null || result == 0) throw new InvalidOperationException("Пользователь не существует");

                trxContext.Commit();

                return true;
            }
            catch
            {
                trxContext.Rollback();
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(EmployeeUpdateDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);

            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                // Проверка уникальности паспорта
                if (employee.Passport is not null)
                {
                    var isPassportUnique = await _employeeRepository.EnsurePassportUniqueForUpdateAsync(
                        employee,
                        trxContext.Connection,
                        trxContext.Transaction
                    );

                    if (!isPassportUnique)
                    {
                        throw new InvalidOperationException("Такой паспорт уже зарегистрирован.");
                    }
                }

                if (employee.Phone is not null)
                {
                    // Проверка уникальности телефона
                    var isPhoneUnique = await _employeeRepository.EnsurePhoneUniqueForUpdateAsync(
                        employee,
                        trxContext.Connection,
                        trxContext.Transaction
                    );

                    if (!isPhoneUnique)
                    {
                        throw new InvalidOperationException("Данный номер уже зарегистрирован.");
                    }
                }

                // Выполняем обновление, если валидации прошли
                await _employeeRepository.UpdateAsync(
                    employee,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return true;
            }
            catch
            {
                trxContext.Rollback();
                return false;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetAllAsync(
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }

        public async Task<EmployeeDto?> GetByIdAsync(int Id)
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetByIdAsync(
                    Id,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<EmployeeDto>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyIdAsync(int companyId)
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetEmployeesByCompanyIdAsync(companyId,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByCompanyNameAsync(string companyName)
        {

            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetEmployeesByCompanyNameAsync(companyName,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentId,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }

        public async Task<IEnumerable<EmployeeDto>> GetEmployeesByDepartmentAsync(string departmentName)
        {
            using var trxContext = await _employeeRepository.BeginTransactionAsync();

            try
            {
                var employees = await _employeeRepository.GetEmployeesByDepartmentAsync(departmentName,
                    trxContext.Connection,
                    trxContext.Transaction
                );

                trxContext.Commit();

                return _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            }
            catch
            {
                trxContext.Rollback();
                return null;
            }
        }
    }
}
