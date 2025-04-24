using Foundation.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Foundation.Repositories
{
    public interface IAuditLogRepository : IRepository<AuditLog, Guid>
    {
    }
}
