using Microsoft.AspNetCore.Mvc;
using Test.DTO.Company;
using Test.Services.Interfaces;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // POST: api/company
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CompanyCreateDto companyDto)
        {
            var id = await _companyService.CreateAsync(companyDto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/company
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CompanyDto companyDto)
        {
            var result = await _companyService.UpdateAsync(companyDto);
            return result ? Ok() : NotFound();
        }

        // DELETE: api/company/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _companyService.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }

        // GET: api/company/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompanyDto>> GetById(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            return company == null ? NotFound() : Ok(company);
        }

        // GET: api/company
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }
    }
}
