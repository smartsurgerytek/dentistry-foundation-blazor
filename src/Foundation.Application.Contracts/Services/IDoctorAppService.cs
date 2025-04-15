using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IDoctorAppService
    {
        Task<List<DoctorDto>> GetDoctorsAsync();
        Task<DoctorDto> GetDoctorAsync(Guid doctorId);
        Task<List<DoctorDto>> GetDoctorsByAsync(Guid departmentId);
        Task CreateDoctorAsync(CreateUpdateDoctorDto input);
        Task UpdateDoctorAsync(Guid doctorId, CreateUpdateDoctorDto input);
        Task DeleteDoctorAsync(Guid doctorId);
    }

}
