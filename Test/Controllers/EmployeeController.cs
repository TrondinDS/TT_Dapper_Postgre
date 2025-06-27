using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.DTO.Employe;
using Test.Services.Interfaces;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EmployeeCreateDto employeeDto)
        {
            var id = await _employeeService.AddEmployeeAsync(employeeDto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EmployeeUpdateDto employeeDto)
        {
            var result = await _employeeService.UpdateEmployeeAsync(employeeDto);
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            return result ? Ok() : NotFound();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDto>> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            return employee == null ? NotFound() : Ok(employee);
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("company/{companyId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByCompanyId(int companyId)
        {
            var employees = await _employeeService.GetEmployeesByCompanyIdAsync(companyId);
            return Ok(employees);
        }

        [HttpGet("company-name")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByCompanyName([FromQuery] string name)
        {
            var employees = await _employeeService.GetEmployeesByCompanyNameAsync(name);
            return Ok(employees);
        }

        // GET: api/employee/department/{departmentId}
        [HttpGet("department/{departmentId:int}")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartmentId(int departmentId)
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(departmentId);
            return Ok(employees);
        }

        // GET: api/employee/department-name?name=HR
        [HttpGet("department-name")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartmentName([FromQuery] string name)
        {
            var employees = await _employeeService.GetEmployeesByDepartmentAsync(name);
            return Ok(employees);
        }
    }
}
