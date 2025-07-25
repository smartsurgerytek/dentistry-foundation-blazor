﻿using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Foundation.Services
{
    public interface IPatientAppService:IApplicationService
    {
        Task<PatientRecordDto> GetPatientRecordByAsync(Guid patientId);
        Task<List<PatientDto>> GetPatientsAsync();
        Task<PatientDto> GetPatientAsync(Guid patientId);        
        Task<List<PatientDto>> GetPatientByAsync(Guid doctorId);
        Task CreatePatientAsync(CreateUpdatePatientDto input);
        Task UpdatePatientAsync(Guid patientId, CreateUpdatePatientDto input);
        Task DeletePatientAsync(Guid patientId);
    }
}
