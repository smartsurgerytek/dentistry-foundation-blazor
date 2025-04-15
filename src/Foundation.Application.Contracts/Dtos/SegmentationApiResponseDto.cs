using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Foundation.Dtos
{
    public class SegmentationApiResponseDto
    {
        [JsonProperty("request_id")]
        public string Request_Id {get; set;}
        [JsonProperty("content_type")]
        public string Content_Type {get; set;}
        [JsonProperty("image")]
        public string Image {get; set;}
        [JsonProperty("messages")]
        public string Messages {get; set;}
    }
}