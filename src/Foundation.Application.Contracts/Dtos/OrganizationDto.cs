using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Foundation.Dtos
{
    public class OrganizationDto : EntityDto<Guid>
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(128, ErrorMessage = "Name cannot exceed 128 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(256, ErrorMessage = "Address cannot exceed 256 characters")]
        public string Address { get; set; }
    }
}
