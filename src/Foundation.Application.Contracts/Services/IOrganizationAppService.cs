using Foundation.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Services
{
    public interface IOrganizationAppService
    {
        Task<List<OrganizationDto>> GetOrganizationsAsync();
        Task<OrganizationDto> GetOrganizationAsync(Guid organizationId);
        Task CreateOrganizationAsync(CreateUpdateOrganizationDto input);
        Task UpdateOrganizationAsync(Guid organizationId, CreateUpdateOrganizationDto input);
        Task DeleteOrganizationAsync(Guid organizationId);
    }

}
