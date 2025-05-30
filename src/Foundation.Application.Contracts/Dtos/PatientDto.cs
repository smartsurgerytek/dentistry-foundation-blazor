using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Foundation.Dtos
{
    public class PatientDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string PatientNumber { get; set; }
        //public string Gender { get; set; }
        public Guid DoctorId { get; set; }
    }
}
