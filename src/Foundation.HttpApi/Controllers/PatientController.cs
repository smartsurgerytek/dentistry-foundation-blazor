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
    [Route("api/patients")]
    [ApiController]
    public class PatientController : FoundationController
    {
        private readonly IPatientAppService _patientAppService;

        public PatientController(IPatientAppService patientAppService)
        {
            _patientAppService = patientAppService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PatientDto>>> GetPatients()
        {
            var patients = await _patientAppService.GetPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatient(Guid id)
        {
            var patient = await _patientAppService.GetPatientAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<ActionResult<List<DoctorDto>>> GetPatientBy(Guid doctorId)
        {
            var patient = await _patientAppService.GetPatientByAsync(doctorId);
            return Ok(patient);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePatient([FromBody] CreateUpdatePatientDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest();
            }

            await _patientAppService.CreatePatientAsync(patientDto);
            //return CreatedAtAction(nameof(GetPatient), new { id = patientDto.Id }, patientDto);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePatient(Guid id, [FromBody] CreateUpdatePatientDto patientDto)
        {
            //if (id != patientDto.Id)
            //{
            //    return BadRequest();
            //}

            await _patientAppService.UpdatePatientAsync(id, patientDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePatient(Guid id)
        {
            await _patientAppService.DeletePatientAsync(id);
            return NoContent();
        }
    }

}
