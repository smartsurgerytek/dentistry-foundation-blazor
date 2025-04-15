using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IDepartmentAppService
    {
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<List<DepartmentDto>> GetDepartmentsByAsync(Guid organizationId);
        Task<DepartmentDto> GetDepartmentAsync(Guid departmentId);
        Task CreateDepartmentAsync(CreateUpdateDepartmentDto input);
        Task UpdateDepartmentAsync(Guid departmentId, CreateUpdateDepartmentDto input);
        Task DeleteDepartmentAsync(Guid departmentId);
    }
}
