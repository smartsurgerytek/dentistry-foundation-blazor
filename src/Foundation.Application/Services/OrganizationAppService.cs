using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Services
{

    public class OrganizationAppService : ApplicationService, IOrganizationAppService, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public OrganizationAppService(IRepository<Organization, Guid> organizationRepository)
        {
            _organizationRepository = organizationRepository;
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync()
        {
            var organizations = await _organizationRepository.GetListAsync();
            return ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(organizations);
        }

        public async Task<OrganizationDto> GetOrganizationAsync(Guid organizationId)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            return ObjectMapper.Map<Organization, OrganizationDto>(organization);
        }

        public async Task CreateOrganizationAsync(CreateUpdateOrganizationDto input)
        {
            var organization = ObjectMapper.Map<CreateUpdateOrganizationDto, Organization>(input);
            await _organizationRepository.InsertAsync(organization);
        }

        public async Task UpdateOrganizationAsync(Guid organizationId, CreateUpdateOrganizationDto input)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            ObjectMapper.Map(input, organization);
            await _organizationRepository.UpdateAsync(organization);
        }

        public async Task DeleteOrganizationAsync(Guid organizationId)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            await _organizationRepository.DeleteAsync(organization);
        }
    }

}