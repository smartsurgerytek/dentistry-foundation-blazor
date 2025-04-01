using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class CreateUpdatePatientDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public DateTime DateOfBirth { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public Guid DoctorId { get; set; }
    }

}
