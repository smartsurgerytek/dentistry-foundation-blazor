using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class PatientRecordDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateOnly PatientDob { get; set; }
    }
}
