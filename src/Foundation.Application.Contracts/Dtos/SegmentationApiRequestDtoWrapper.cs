using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Foundation.Dtos
{
    public class SegmentationApiRequestDtoWrapper
    {
        public bool IsPeriapicalImage { get; set; }
        public SegmentationApiRequestDto? SegmentationApiRequest { get; set; }
    }
}