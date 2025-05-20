using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class TestMainUser : AuditedAggregateRoot<Guid>
    {
        public string FirstName { get; set; }
        public string Age { get; set; }
        public string Phone { get; set; }
    }
}
