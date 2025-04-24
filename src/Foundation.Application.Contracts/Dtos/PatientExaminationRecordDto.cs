using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class PatientExaminationRecordDto
    {
        public string PatientId { get; set; }
        public List<ToothInfoDto> MaxillaTeeth { get; set; }
        public List<ToothInfoDto> MandibleTeeth { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateOnly PatientDob { get; set; }
        
    }
}
