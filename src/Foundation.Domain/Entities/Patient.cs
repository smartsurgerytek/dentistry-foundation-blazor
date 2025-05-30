using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;


namespace Foundation.Entities
{
    public class Patient : AuditedAggregateRoot<Guid>
    {
        
        public string PatientNumber { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        //public string Gender { get; set; }

        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public ICollection<Record> Records { get; set; }
    }


}