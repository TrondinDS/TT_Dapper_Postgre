using Test.DTO.Company;

namespace Test.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<int> CreateAsync(CompanyCreateDto dto);
        Task<bool> UpdateAsync(CompanyDto companyDto);
        Task<bool> DeleteAsync(int id);
        Task<CompanyDto?> GetByIdAsync(int id);
        Task<IEnumerable<CompanyDto>> GetAllAsync();
    }
}
