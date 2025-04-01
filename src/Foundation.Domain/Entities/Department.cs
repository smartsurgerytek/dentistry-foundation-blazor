using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class Department : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public ICollection<Doctor> Doctors { get; set; }
    }


}
