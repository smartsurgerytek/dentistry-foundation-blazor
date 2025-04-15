using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class Organization : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<Department> Departments { get; set; }
    }
}
