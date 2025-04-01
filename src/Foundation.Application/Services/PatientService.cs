using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Foundation.Services
{
    public class PatientAppService : ApplicationService, IPatientAppService, ITransientDependency
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;

        public PatientAppService(IRepository<Patient, Guid> patientRepository, IRepository<Doctor, Guid> doctorRepository)
        {
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<List<PatientDto>> GetPatientsAsync()
        {
            var patients = await _patientRepository.GetListAsync();
            return ObjectMapper.Map<List<Patient>, List<PatientDto>>(patients);
        }

        public async Task<PatientDto> GetPatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                throw new EntityNotFoundException(typeof(Patient), patientId);
            }
            return ObjectMapper.Map<Patient, PatientDto>(patient);
        }

        public async Task CreatePatientAsync(CreateUpdatePatientDto input)
        {
            var patient = ObjectMapper.Map<CreateUpdatePatientDto, Patient>(input);
            await _patientRepository.InsertAsync(patient);
        }

        public async Task UpdatePatientAsync(Guid patientId, CreateUpdatePatientDto input)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                throw new EntityNotFoundException(typeof(Patient), patientId);
            }

            ObjectMapper.Map(input, patient);
            await _patientRepository.UpdateAsync(patient);
        }

        public async Task DeletePatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                throw new EntityNotFoundException(typeof(Patient), patientId);
            }

            await _patientRepository.DeleteAsync(patient);
        }

        public async Task<List<PatientDto>> GetPatientByAsync(Guid doctorId)
        {
            var patient = await _patientRepository.GetListAsync(x => x.DoctorId == doctorId);            
            return ObjectMapper.Map<List<Patient>,List<PatientDto>>(patient);
        }
    }

}
