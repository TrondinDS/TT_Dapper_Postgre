using AutoMapper;
using Test.DB.Models;
using Test.DTO.Company;
using Test.Repositories.Interfaces;
using Test.Services.Interfaces;

namespace Test.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CompanyCreateDto dto)
        {
            var company = _mapper.Map<Company>(dto);
            return await _companyRepository.AddAsync(company);
        }

        public async Task<bool> UpdateAsync(CompanyDto companyDto)
        {
            var company = _mapper.Map<Company>(companyDto);
            return await _companyRepository.UpdateAsync(company);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _companyRepository.DeleteAsync(id);
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }

        public async Task<IEnumerable<CompanyDto>> GetAllAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }
    }
}
