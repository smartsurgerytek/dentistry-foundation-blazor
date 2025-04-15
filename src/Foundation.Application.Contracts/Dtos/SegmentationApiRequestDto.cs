using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Foundation.Dtos
{
    public class SegmentationApiRequestDto
    {
        [JsonProperty("image")]
        public string Image {get; set;}
        [JsonProperty("scale_x")]
        public double Scale_X { get; set; } = 0.0234;
        [JsonProperty("scale_y")]
        public double Scale_Y { get; set; } = 0.041;
    }
}