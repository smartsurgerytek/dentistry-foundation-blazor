using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace Foundation.Entities
{
    public class Record : AuditedAggregateRoot<Guid>
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
    }
}
