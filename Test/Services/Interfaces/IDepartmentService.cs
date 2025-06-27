using Test.DTO.Departament;

namespace Test.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartamentDtoRD>> GetAllAsync();
        Task<DepartamentDtoRD?> GetByIdAsync(int id);
        Task<int> CreateAsync(DepartamentCreateDto departmentCreateDto);
        Task<bool> UpdateAsync(DepartamentDtoRD departmentDto);
        Task<bool> DeleteAsync(int id);
    }
}
