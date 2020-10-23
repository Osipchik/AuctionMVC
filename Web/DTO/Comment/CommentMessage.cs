using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Web.DTO.Comment
{
    public class CommentMessage
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("lotId")]
        public string LotId { get; set; }
    }
}