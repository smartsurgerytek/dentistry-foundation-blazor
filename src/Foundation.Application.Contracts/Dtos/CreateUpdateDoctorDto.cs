using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class CreateUpdateDoctorDto
    {
        public Guid Id { get; set; }



        [Required(ErrorMessage = "Doctor Name is required")]
        [StringLength(100, ErrorMessage = "Name must be under 100 characters")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Specialty is required")]
        [StringLength(100, ErrorMessage = "Specialty must be under 100 characters")]

        public string Specialty { get; set; }
        public Guid DepartmentId { get; set; }
    }

}
