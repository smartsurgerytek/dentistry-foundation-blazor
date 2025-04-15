using Newtonsoft.Json;

namespace Foundation.Dtos
{
    public class PaPanoClassificationRequestDto
    {
        [JsonProperty("image")]
        public string Image {get; set;}
    }
}