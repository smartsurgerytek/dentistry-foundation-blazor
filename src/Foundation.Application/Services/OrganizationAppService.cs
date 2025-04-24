using Foundation.Dtos;
using Foundation.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.AspNetCore.Controllers;
using Volo.Abp.AspNetCore.Mvc.ApplicationConfigurations;
using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Services
{    
    public class OrganizationAppService : ApplicationService, IOrganizationAppService, ITransientDependency
    {
        private readonly IRepository<Organization, Guid> _organizationRepository;
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;

        public OrganizationAppService(IRepository<Organization, Guid> organizationRepository,
            IRepository<AuditLog, Guid> auditLogRepository)
        {
            _organizationRepository = organizationRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<List<OrganizationDto>> GetOrganizationsAsync()
        {            
            var organizations = await _organizationRepository.GetListAsync();

            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(OrganizationAppService),
                MethodName = "Organization - GetOrganizations",
                Parameters = JsonSerializer.Serialize(string.Empty),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });

            return ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(organizations);
        }

        public async Task<OrganizationDto> GetOrganizationAsync(Guid organizationId)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(OrganizationAppService),
                MethodName = "Organization - GetOrganization",
                Parameters = JsonSerializer.Serialize(organizationId),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });
            return ObjectMapper.Map<Organization, OrganizationDto>(organization);
        }

        public async Task CreateOrganizationAsync(CreateUpdateOrganizationDto input)
        {
            var organization = ObjectMapper.Map<CreateUpdateOrganizationDto, Organization>(input);
            await _organizationRepository.InsertAsync(organization);

            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(OrganizationAppService),
                MethodName = "Organization - CreateOrganization",
                Parameters = JsonSerializer.Serialize(input),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });

        }

        public async Task UpdateOrganizationAsync(Guid organizationId, CreateUpdateOrganizationDto input)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            ObjectMapper.Map(input, organization);

            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(OrganizationAppService),
                MethodName = "Organization - UpdateOrganization",
                Parameters = JsonSerializer.Serialize(input),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });

            await _organizationRepository.UpdateAsync(organization);
        }

        public async Task DeleteOrganizationAsync(Guid organizationId)
        {
            var organization = await _organizationRepository.FirstOrDefaultAsync(x => x.Id == organizationId, CancellationToken.None);
            if (organization == null)
            {
                throw new EntityNotFoundException(typeof(Organization), organizationId);
            }

            await _auditLogRepository.InsertAsync(new AuditLog(GuidGenerator.Create())
            {
                UserName = CurrentUser.UserName ?? "System",
                ServiceName = nameof(OrganizationAppService),
                MethodName = "Organization - DeleteOrganizationA",
                Parameters = JsonSerializer.Serialize(organizationId),
                ExecutionTime = Clock.Now,
                ExecutionDuration = 150
            });

            await _organizationRepository.DeleteAsync(organization);
        }
    }
}