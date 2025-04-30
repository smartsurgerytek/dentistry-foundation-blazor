using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace Foundation.Entities
{
    public class AuditLog : AggregateRoot<Guid>
    {
        public string UserName { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public DateTime ExecutionTime { get; set; }
        public int ExecutionDuration { get; set; }

        public AuditLog(Guid id) : base(id) { }
    }
}
