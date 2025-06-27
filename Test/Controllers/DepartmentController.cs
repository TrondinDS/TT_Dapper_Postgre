using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Test.DTO.Departament;
using Test.Services.Interfaces;

namespace Test.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartamentDtoRD>>> GetAll()
        {
            var departments = await _departmentService.GetAllAsync();
            return Ok(departments);
        }

        // GET: api/department/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartamentDtoRD>> GetById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            return department == null ? NotFound() : Ok(department);
        }

        // POST: api/department
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DepartamentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var id = await _departmentService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT: api/department/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] DepartamentDtoRD dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _departmentService.UpdateAsync(dto);
            return success ? Ok() : NotFound();
        }

        // DELETE: api/department/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _departmentService.DeleteAsync(id);
            return success ? Ok() : NotFound();
        }
    }
}
