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
    [Route("api/doctors")]
    [ApiController]
    public class DoctorController : FoundationController
    {
        private readonly IDoctorAppService _doctorAppService;

        public DoctorController(IDoctorAppService doctorAppService)
        {
            _doctorAppService = doctorAppService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DoctorDto>>> GetDoctors()
        {
            var doctors = await _doctorAppService.GetDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDto>> GetDoctor(Guid id)
        {
            var doctor = await _doctorAppService.GetDoctorAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            return Ok(doctor);
        }

        [HttpGet("by-department/{departmentId}")]
        public async Task<ActionResult<List<DoctorDto>>> GetDoctorsBy(Guid departmentId)
        {
            var doctors = await _doctorAppService.GetDoctorsByAsync(departmentId);
            return Ok(doctors);
        }


        [HttpPost]
        public async Task<ActionResult> CreateDoctor([FromBody] CreateUpdateDoctorDto doctorDto)
        {
            if (doctorDto == null)
            {
                return BadRequest();
            }

            await _doctorAppService.CreateDoctorAsync(doctorDto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDoctor(Guid id, [FromBody] CreateUpdateDoctorDto doctorDto)
        {
            await _doctorAppService.UpdateDoctorAsync(id, doctorDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDoctor(Guid id)
        {
            await _doctorAppService.DeleteDoctorAsync(id);
            return NoContent();
        }
    }

}
