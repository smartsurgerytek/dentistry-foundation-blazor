using Newtonsoft.Json;

namespace Foundation.Dtos
{
    public class PaPanoClassificationResponseDto
    {
        [JsonProperty("request_id")]
        public string Request_Id {get; set;}
        [JsonProperty("predicted_class")]
        public string Predicted_Class {get; set;}
        [JsonProperty("scores")]
        public string Scores {get; set;}
        [JsonProperty("message")]
        public string Message {get; set;}
    }
}