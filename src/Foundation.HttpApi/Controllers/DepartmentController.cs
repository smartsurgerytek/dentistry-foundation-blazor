using Foundation.Dtos;
using Foundation.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Controllers
{
    [Route("api/departments")]
    [ApiController]
    public class DepartmentController : FoundationController
    {
        private readonly IDepartmentAppService _departmentAppService;

        public DepartmentController(IDepartmentAppService departmentAppService)
        {
            _departmentAppService = departmentAppService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartments()
        {
            var departments = await _departmentAppService.GetDepartmentsAsync();
            return Ok(departments);
        }

        [HttpGet("by-organization/{organizationid}")]       
        public async Task<ActionResult<List<DepartmentDto>>> GetDepartmentsBy(Guid organizationid)
        {
            var departments = await _departmentAppService.GetDepartmentsByAsync(organizationid);
            return Ok(departments);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDto>> GetDepartment(Guid id)
        {
            var department = await _departmentAppService.GetDepartmentAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult> CreateDepartment([FromBody] CreateUpdateDepartmentDto departmentDto)
        {
            if (departmentDto == null)
            {
                return BadRequest();
            }

            await _departmentAppService.CreateDepartmentAsync(departmentDto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDepartment(Guid id, [FromBody] CreateUpdateDepartmentDto departmentDto)
        {
            await _departmentAppService.UpdateDepartmentAsync(id, departmentDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(Guid id)
        {
            await _departmentAppService.DeleteDepartmentAsync(id);
            return NoContent();
        }
    }

}
