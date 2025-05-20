using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class TestUser : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Mobile { get; set; }        
    }
}
