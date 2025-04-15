using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Dtos
{
    public class ToothInfoDto
    {
        public int ToothNumber { get; set; }
        public bool CariesYes { get; set; }
        public bool CariesNo { get; set; }
        public string SelectedPE { get; set; }
        public string Description { get; set; }
    }
}
