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
        public List<ToothInfoDto> UpperLeft { get; set; }
        public List<ToothInfoDto> UpperRight { get; set; }
        public List<ToothInfoDto> LowerLeft { get; set; }
        public List<ToothInfoDto> LowerRight { get; set; }
        public List<ToothInfoDto> MaxillaTeeth { get; set; }
        public List<ToothInfoDto> MandibleTeeth { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateOnly PatientDob { get; set; }
        public string FileBaseAddress { get; set; }

        public string ImageNames { get; set; }
    }
}
