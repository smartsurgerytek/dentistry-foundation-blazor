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
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string OrganizationName { get; set; }
        public string DepartmentName { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
