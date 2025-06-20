using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class CreateUpdateRecordDto
    {
        public Guid PatientId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string FileName { get; set; }
        public Guid Id { get; set; }
    }
}
