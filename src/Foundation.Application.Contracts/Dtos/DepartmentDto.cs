using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Foundation.Dtos
{
    public class DepartmentDto : EntityDto<Guid>
    {

        [Required(ErrorMessage = "Department name is required.")]
        [StringLength(100)]
        public string Name { get; set; }
        public Guid OrganizationId { get; set; }
    }

}
