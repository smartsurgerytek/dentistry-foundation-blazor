using AutoMapper.Internal.Mappers;
using Foundation.Dtos;
using Foundation.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;

        public DoctorAppService(
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<AuditLog, Guid> auditLogRepository)
        {
            _doctorRepository = doctorRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<DoctorDto>> GetDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetListAsync();

            await LogAudit("GetDoctors", string.Empty);

            return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(doctors);
        }

        public async Task<DoctorDto> GetDoctorAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }

            await LogAudit("GetDoctor", doctorId);

            return ObjectMapper.Map<Doctor, DoctorDto>(doctor);
        }

        public async Task CreateDoctorAsync(CreateUpdateDoctorDto input)
        {
            var doctor = ObjectMapper.Map<CreateUpdateDoctorDto, Doctor>(input);
            await _doctorRepository.InsertAsync(doctor);

            await LogAudit("CreateDoctor", input);
        }

        public async Task UpdateDoctorAsync(Guid doctorId, CreateUpdateDoctorDto input)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }

            ObjectMapper.Map(input, doctor);
            await _doctorRepository.UpdateAsync(doctor);

            await LogAudit("UpdateDoctor", input);
        }

        public async Task DeleteDoctorAsync(Guid doctorId)
        {
            var doctor = await _doctorRepository.FirstOrDefaultAsync(x => x.Id == doctorId);
            if (doctor == null)
            {
                throw new EntityNotFoundException(typeof(Doctor), doctorId);
            }

            await _doctorRepository.DeleteAsync(doctor);

            await LogAudit("DeleteDoctor", doctorId);
        }

        public async Task<List<DoctorDto>> GetDoctorsByAsync(Guid departmentId)
        {
            var doctors = await _doctorRepository.GetListAsync(x => x.DepartmentId == departmentId);

            await LogAudit("GetDoctorsBy", departmentId);

            return ObjectMapper.Map<List<Doctor>, List<DoctorDto>>(doctors);
        }

        private async Task LogAudit(string methodName, object parameters)
        {
            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(DoctorAppService),
                MethodName = $"Doctor - {methodName}",
                Parameters = JsonSerializer.Serialize(parameters),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
        }
    }


}
