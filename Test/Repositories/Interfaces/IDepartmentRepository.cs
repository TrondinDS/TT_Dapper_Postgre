using Test1.DB.Models;

namespace Test.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllAsync();
        Task<int> AddAsync(Department department);
        Task<bool> UpdateAsync(Department department);
        Task<bool> DeleteAsync(int id);
    }
}
