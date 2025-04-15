using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Foundation.Dtos
{
    public class DoctorDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string Specialty { get; set; }
        public Guid DepartmentId { get; set; }
    }

}
