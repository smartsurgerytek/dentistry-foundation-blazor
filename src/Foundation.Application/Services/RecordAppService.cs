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
using Volo.Abp.ObjectMapping;

namespace Foundation.Services
{
    [RemoteService(Name = "Record")]
    [Route("api/record")]
    public class RecordAppService : ApplicationService, IRecordAppService, ITransientDependency
    {
        private readonly IRepository<Record, Guid> _recordRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;

        public RecordAppService(
            IRepository<Record, Guid> recordRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<AuditLog, Guid> auditLogRepository)
        {
            _recordRepository = recordRepository;
            _patientRepository = patientRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<RecordDto>> GetRecordsAsync()
        {
            var queryable = await _recordRepository.GetQueryableAsync();

            var records = queryable
                .Select(r => new RecordDto
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    PatientName = r.Patient != null ? r.Patient.Name : "Unknown",
                    CreatedDate = r.CreatedDate,
                    DoctorName = r.Patient.Doctor != null ? r.Patient.Doctor.Name : "Unknown",
                    OrganizationName = r.Patient.Doctor.Department.Organization.Name ?? "Unknown",
                    DepartmentName = r.Patient.Doctor.Department.Name ?? "Unknown",
                    FileName = r.FileName
                })
                .ToList();

            await LogAudit("GetRecords", string.Empty);

            return records;
        }

        public async Task<RecordDto> GetRecordAsync(Guid recordId)
        {
            var queryable = await _recordRepository.GetQueryableAsync();

            var record = queryable
                .Where(r => r.Id == recordId)
                .Select(r => new RecordDto
                {
                    Id = r.Id,
                    PatientId = r.PatientId,
                    PatientName = r.Patient != null ? r.Patient.Name : "Unknown",
                    CreatedDate = r.CreatedDate
                })
                .FirstOrDefault();

            if (record == null)
            {
                throw new BusinessException("Record not found!");
            }

            await LogAudit("GetRecord", recordId);

            return record;
        }

        public async Task CreateRecordAsync(CreateUpdateRecordDto input)
        {
            var record = await _recordRepository.FirstOrDefaultAsync(x => x.PatientId == input.PatientId);

            if(record==null)
            {
                var recordMain = ObjectMapper.Map<CreateUpdateRecordDto, Record>(input);
                await _recordRepository.InsertAsync(recordMain);
                await LogAudit("CreateRecord", input);
            }
            else
            {
                ObjectMapper.Map(input, record);
                await _recordRepository.UpdateAsync(record);
                await LogAudit("UpdateRecord", input);
            }                
        }

        public async Task UpdateRecordAsync(Guid recordId, CreateUpdateRecordDto input)
        {
            var record = await _recordRepository.FirstOrDefaultAsync(x => x.Id == recordId);
            if (record == null)
            {
                throw new BusinessException("Record not found!");
            }

            ObjectMapper.Map(input, record);
            await _recordRepository.UpdateAsync(record);

            await LogAudit("UpdateRecord", input);
        }

        public async Task DeleteRecordAsync(Guid recordId)
        {
            var record = await _recordRepository.FirstOrDefaultAsync(x => x.Id == recordId);
            if (record == null)
            {
                throw new BusinessException("Record not found!");
            }

            await _recordRepository.DeleteAsync(record);

            await LogAudit("DeleteRecord", recordId);
        }

        private async Task LogAudit(string methodName, object parameters)
        {
            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(RecordAppService),
                MethodName = $"Record - {methodName}",
                Parameters = JsonSerializer.Serialize(parameters),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
        }
    }


}
