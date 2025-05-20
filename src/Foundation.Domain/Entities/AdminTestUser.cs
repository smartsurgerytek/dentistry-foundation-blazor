using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class AdminTestUser : AuditedAggregateRoot<Guid>
    {
        public string LastName { get; set; }        
        public string Mobile { get; set; }
    }
}
