using Foundation.Dtos;
using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Services
{
    public class AuditLogAppServices : ApplicationService, ITransientDependency
    {
        private readonly IRepository<AuditLog, Guid> _auditLogRepository;

        public AuditLogAppServices(IRepository<AuditLog, Guid> auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }
        
        public async Task<List<AuditLogDto>> GetAuditLogAsync()
        {
            var auditLogs = await _auditLogRepository.GetListAsync();
            return ObjectMapper.Map<List<AuditLog>, List<AuditLogDto>>(auditLogs);
        }

        public async Task InsertAuditLogAsync(AuditLog auditLog)
        {
            await _auditLogRepository.InsertAsync(auditLog);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}
