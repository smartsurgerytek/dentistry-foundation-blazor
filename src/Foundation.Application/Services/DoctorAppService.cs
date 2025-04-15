using AutoMapper.Internal.Mappers;
using Foundation.Dtos;
using Foundation.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Services
{
    [RemoteService(Name = "Doctor")]
    [Route("api/doctor")]
    public class DoctorAppService : ApplicationService, IDoctorAppService, ITransientDependency
    {
        private readonly IRepository<Doctor, Guid> _doctorRepository;

        public DoctorAppService(IRepository<Doctor, Guid> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<List<DoctorDto>> GetDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetListAsync();
            return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> GetDoctorAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId, CancellationToken.None);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }
            return ObjectMapper.Map<Doctor, DoctorDto>(doctor);
        }

        public async Task CreateDoctorAsync(CreateUpdateDoctorDto input)
        {
            var doctor = ObjectMapper.Map<CreateUpdateDoctorDto, Doctor>(input);
            await _doctorRepository.InsertAsync(doctor);
        }

        public async Task UpdateDoctorAsync(Guid doctorId, CreateUpdateDoctorDto input)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId, CancellationToken.None);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }

            ObjectMapper.Map(input, doctor);
            await _doctorRepository.UpdateAsync(doctor);
        }

        public async Task DeleteDoctorAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId, CancellationToken.None);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }

            await _doctorRepository.DeleteAsync(doctor);
        }

        public async Task<List<DoctorDto>> GetDoctorsByAsync(Guid departmentId)
        {
            var doctors = await _doctorRepository.GetListAsync(x => x.DepartmentId == departmentId);
            return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(doctors);
        }
    }

}
