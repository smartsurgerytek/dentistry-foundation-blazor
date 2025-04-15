using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IPatientAppService
    {
        Task<PatientRecordDto> GetPatientReportRecordByAsync(Guid patientId);
        Task<List<PatientDto>> GetPatientsAsync();
        Task<PatientDto> GetPatientAsync(Guid patientId);        
        Task<List<PatientDto>> GetPatientByAsync(Guid doctorId);
        Task CreatePatientAsync(CreateUpdatePatientDto input);
        Task UpdatePatientAsync(Guid patientId, CreateUpdatePatientDto input);
        Task DeletePatientAsync(Guid patientId);
    }
}
