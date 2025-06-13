using Foundation.Dtos;
using Foundation.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Amazon;
using Amazon.S3.Model;
using Amazon.S3;
using System.IO;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Foundation.Services
{
    [RemoteService(Name = "Patient")]
    [Route("api/patient")]
    public class PatientAppService : ApplicationService, IPatientAppService, ITransientDependency
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;
        private readonly IRepository<Department, Guid> _departmentRepository;
        private readonly IRepository<Organization, Guid> _organizationRepository;        
        private string accessKey = "AKIAZI2LGNNVDTFYF57P";
        private string secretKey = "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";

        public PatientAppService(
            IRepository<Patient, Guid> patientRepository,
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<AuditLog, Guid> auditLogRepository,
            IRepository<Department, Guid> departmentRepository,
        IRepository<Organization, Guid> organizationRepository
            )
        {
            _patientRepository = patientRepository;
            _doctorRepository = doctorRepository;
            _auditLogRepository = auditLogRepository;
            _departmentRepository = departmentRepository;
            _organizationRepository = organizationRepository;            
        }

        public async Task<List<PatientDto>> GetPatientsAsync()
        {
            var patients = await _patientRepository.GetListAsync();

            await LogAudit("GetPatients", string.Empty);

            return ObjectMapper.Map<List<Patient>, List<PatientDto>>(patients);
        }

        public async Task<PatientDto> GetPatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                throw new EntityNotFoundException(typeof(Patient), patientId);
            }

            await LogAudit("GetPatient", patientId);

            return ObjectMapper.Map<Patient, PatientDto>(patient);
        }

        public async Task CreatePatientAsync(CreateUpdatePatientDto input)
        {

            var patient = ObjectMapper.Map<CreateUpdatePatientDto, Patient>(input);
            var patientData = await _patientRepository.InsertAsync(patient);

            var doctor = await _doctorRepository.GetAsync(patientData.DoctorId);
            var department = await _departmentRepository.GetAsync(doctor.DepartmentId);
            var organization = await _organizationRepository.GetAsync(department.OrganizationId);

            string fName = "foundation";
            //string org = organization.Name.Replace(" ", "_");
            //string dept = department.Name.Replace(" ", "_");
            //string doc = doctor.Name.Replace(" ","_");
            string patientFolder = $"{patientData.Name.Replace(" ", "_")}_{patientData.Id}";

            var fullPath = $"{fName}/{"Patient"}/{patientFolder}/";
            var bucketName = "smartsurgerytek.foundation";

            using var s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.USWest2);

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fullPath,
                ContentBody = "This is a placeholder file to make the folder visible in S3 console."
            };

            // await s3Client.PutObjectAsync(putRequest);

            await LogAudit("CreatePatient", input);
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

            await LogAudit("UpdatePatient", input);
        }

        public async Task DeletePatientAsync(Guid patientId)
        {
            var patient = await _patientRepository.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null)
            {
                throw new EntityNotFoundException(typeof(Patient), patientId);
            }

            await _patientRepository.DeleteAsync(patient);

            await LogAudit("DeletePatient", patientId);
        }

        public async Task<List<PatientDto>> GetPatientByAsync(Guid doctorId)
        {
            var patient = await _patientRepository.GetListAsync(x => x.DoctorId == doctorId);

            await LogAudit("GetPatientBy", doctorId);

            return ObjectMapper.Map<List<Patient>, List<PatientDto>>(patient);
        }

        public async Task<PatientRecordDto> GetPatientRecordByAsync(Guid patientId)
        {
            var queryable = await _patientRepository.GetQueryableAsync();

            var patient = queryable
                .Where(p => p.Id == patientId)
                .Select(p => new PatientRecordDto
                {
                    PatientId = p.Id,
                    PatientName = p.Name,
                    DoctorName = p.Doctor != null ? p.Doctor.Name : "N/A",
                    PatientDob = p.DateOfBirth
                })
                .FirstOrDefault();

            if (patient == null)
            {
                throw new BusinessException("Patient not found!");
            }

            await LogAudit("GetPatientRecordBy", patientId);

            return patient;
        }

        private async Task LogAudit(string methodName, object parameters)
        {
            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(PatientAppService),
                MethodName = $"Patient - {methodName}",
                Parameters = JsonSerializer.Serialize(parameters),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
        }
    }


}
