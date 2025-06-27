using AutoMapper;
using Test.DTO.Departament;
using Test.Repositories.Interfaces;
using Test.Services.Interfaces;
using Test1.DB.Models;

namespace Test.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper)
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DepartamentDtoRD>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DepartamentDtoRD>>(departments);
        }

        public async Task<DepartamentDtoRD?> GetByIdAsync(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);
            if (department == null) return null;

            return _mapper.Map<DepartamentDtoRD>(department);
        }

        public async Task<int> CreateAsync(DepartamentCreateDto departmentCreateDto)
        {
            var department = _mapper.Map<Department>(departmentCreateDto);

            if (department.CompanyId == 0)
                throw new ArgumentException("CompanyId is required to create Department.");

            return await _departmentRepository.AddAsync(department);
        }

        public async Task<bool> UpdateAsync(DepartamentDtoRD departmentDto)
        {
            if (departmentDto.Id == null)
                return false;
            var existingDepartment = await _departmentRepository.GetByIdAsync((int)departmentDto.Id);
            if (existingDepartment == null)
                return false;

            existingDepartment.Name = departmentDto.Name;
            existingDepartment.Phone = departmentDto.Phone;

            return await _departmentRepository.UpdateAsync(existingDepartment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _departmentRepository.DeleteAsync(id);
        }

    }
}
