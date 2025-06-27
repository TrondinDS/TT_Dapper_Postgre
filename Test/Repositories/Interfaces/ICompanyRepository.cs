using Test.DB.Models;

namespace Test.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetAllAsync();
        Task<int> AddAsync(Company company);
        Task<bool> UpdateAsync(Company company);
        Task<bool> DeleteAsync(int id);
    }
}
