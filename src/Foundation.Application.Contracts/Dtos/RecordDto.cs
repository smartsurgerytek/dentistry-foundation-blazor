using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Foundation.Dtos
{
    public class RecordDto : EntityDto<Guid>
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }        
        public DateTime RecordDate { get; set; }        
        public string? Diagnosis { get; set; }
        public string? Notes { get; set; }
    }
}
