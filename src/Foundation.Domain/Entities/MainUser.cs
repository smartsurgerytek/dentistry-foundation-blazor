using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class MainUser : AuditedAggregateRoot<Guid>
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }        
    }
}
