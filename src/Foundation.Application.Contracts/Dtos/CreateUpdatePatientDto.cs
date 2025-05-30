using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class CreateUpdatePatientDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Patient name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "Patient Number is required.")]
        public string PatientNumber { get; set; }

        //[Required(ErrorMessage = "Gender is required.")]
        //public string Gender { get; set; }

        [Required]
        public Guid DoctorId { get; set; }
    }

}
